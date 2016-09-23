using InvertedTomato.Buffers;
using InvertedTomato.Testable.Sockets;
using InvertedTomato.Testable.Streams;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using ThreePlay.IO.Feather;

namespace InvertedTomato.Net.Feather {
    public abstract class ConnectionBase<TEncoder, TDecoder> 
        where TDecoder : IDecoder, new() 
        where TEncoder : IEncoder, new() {

        /// <summary>
        /// The remote endpoint.
        /// </summary>
        public EndPoint RemoteEndPoint {
            get {
                var clientSocket = ClientSocket;
                if (null == clientSocket) {
                    return null;
                }
                return clientSocket.RemoteEndPoint;
            }
        }

        /// <summary>
        /// The total amount of data transmitted (excluding headers).
        /// </summary>
        public long TotalTxBytes { get; private set; }

        /// <summary>
        /// The total amount of data received (excluding headers).
        /// </summary>
        public long TotalRxBytes { get; private set; }

        /// <summary>
        /// If the connection has been disposed (disconnected).
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Callback for when disconnection takes place.
        /// </summary>
        public Action<DisconnectionType> OnDisconnected; // TODO: consider using an abstract method instead

        /// <summary>
        /// Number of payloads that haven't made it to the TCP buffer yet. Will be used for back-pressure indication.
        /// </summary>
        private int OutstandingSends = 0;

        /// <summary>
        /// Configuration options
        /// </summary>
        private ConnectionOptions Options;

        /// <summary>
        /// Timer to send keep-alive payloads to prevent disconnection.
        /// </summary>
        private System.Timers.Timer KeepAliveTimer;

        /// <summary>
        /// Client socket.
        /// </summary>
        private ISocket ClientSocket;

        /// <summary>
        /// Client stream.
        /// </summary>
        private IStream ClientStream;

        /// <summary>
        /// Receive buffers.
        /// </summary>
        private Buffer<byte> ReceiveBuffer;

        private TDecoder NextPayload;

        /// <summary>
        /// Start the connection. Can only be called once.
        /// </summary>
        public void Start(bool isServerConnection, ISocket clientSocket, ConnectionOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
            if (null == clientSocket) {
                throw new ArgumentNullException("clientSocket");
            }
            if (null != ClientSocket) {
                throw new InvalidOperationException("Already started.");
            }
#endif

            // Store options
            Options = options;

            // Store and setup socket
            ClientSocket = clientSocket;
            ClientSocket.ReceiveBufferSize = Options.ReceiveBufferSize;
            ClientSocket.SendBufferSize = Options.SendBufferSize;
            ClientSocket.LingerState = Options.Linger;
            ClientSocket.NoDelay = Options.NoDelay;

            // Get stream
            if (!Options.IsSecure) {
                ClientStream = ClientSocket.GetStream();
            } else if (isServerConnection) {
                ClientStream = ClientSocket.GetSecureServerStream(Options.ServerCertificate);
            } else {
                ClientStream = ClientSocket.GetSecureClientStream(Options.ServerCommonName, ValidateServerCertificate);
            }

            // Setup keep alive
            if (options.ApplicationLayerKeepAlive) {
                // Start keep-alive timer (must be before receive start)
                KeepAliveTimer = new System.Timers.Timer(options.KeepAliveInterval.TotalMilliseconds);
                KeepAliveTimer.Elapsed += KeepAliveTimer_OnElapsed;
                KeepAliveTimer.Start();
            } else {
                // Enable TCP keep-alive
                ClientSocket.SetKeepAlive(true, options.KeepAliveInterval);
            }

            // Setup receive
            ReceiveBuffer = new Buffer<byte>(1);
            NextPayload = new TDecoder();

            // Seed receiving
            ReceiveBegin();
        }

        /// <summary>
        /// Send single payload to remote endpoint.
        /// </summary>    
        protected void Send(TEncoder payload) {
#if DEBUG
            if (null == payload) {
                throw new ArgumentNullException("payload");
            }
#endif

            RawSend(payload.ToBuffer(), null);
        }

        /// <summary>
        /// Send single payload to remote endpoint and execute a callback when done.
        /// </summary>
        protected void Send(TEncoder payload, Action done) {
#if DEBUG
            if (null == payload) {
                throw new ArgumentNullException("payload");
            }
#endif

            RawSend(payload.ToBuffer(), done);
        }

        /// <summary>
        /// Send multiple payloads to remote endpoint.
        /// </summary>    
        protected void Send(TEncoder[] payloads) {
#if DEBUG
            if (null == payloads) {
                throw new ArgumentNullException("payload");
            }
#endif
            
            Send(payloads, null);
        }

        /// <summary>
        /// Send multiple payloads to remote endpoint and execute a callback when done.
        /// </summary>
        protected void Send(TEncoder[] payloads, Action done) {
#if DEBUG
            if (null == payloads) {
                throw new ArgumentNullException("payloads");
            }
            foreach (var payload in payloads) {
                if (null == payload) {
                    throw new ArgumentNullException("payloads", "Element in array.");
                }
            }
#endif

            // Get buffers
            var buffers = payloads.Select(a => a.ToBuffer());

            // Merge into master buffer
            var masterLength = buffers.Sum(a => a.Used);
            var masterBuffer = new Buffer<byte>(masterLength);
            buffers.Each(a => masterBuffer.EnqueueBuffer(a));

            // Send in one batch
            RawSend(masterBuffer, done);
        }

        private void RawSend(ReadOnlyBuffer<byte> buffer, Action done) {
            // Increment outstanding counter
            Interlocked.Increment(ref OutstandingSends);

            // Send
            try {
                ClientStream.BeginWrite(buffer.GetUnderlying(), buffer.Start, buffer.Used, (ar) => {
                    try {
                        // Complete send
                        ClientStream.EndWrite(ar);
                    } catch (ObjectDisposedException) {
                    } catch (IOException) {
                        // Report connection failure
                        DisconnectInner(DisconnectionType.ConnectionInterupted);
                        return;
                    } finally {
                        // Update total-TX counter
                        TotalTxBytes = unchecked(TotalTxBytes + buffer.Used);

                        // Decrement outstanding counter
                        Interlocked.Decrement(ref OutstandingSends);

                        // Callback success
                        done.TryInvoke();
                    }
                }, null);
            } catch (ObjectDisposedException) {
            } catch (IOException) {
                // Report connection failure
                DisconnectInner(DisconnectionType.ConnectionInterupted);
            }

            // Restart keep-alive timer
            if (Options.ApplicationLayerKeepAlive) {
                KeepAliveTimer.Restart();
            }
        }

        /// <summary>
        /// When a payload arrives.
        /// </summary>
        /// <param name="payload"></param>
        protected abstract void OnDataArrived(Buffer<byte> payload);

        /// <summary>
        /// Disconnect from the remote endpoint and dispose.
        /// </summary>
        public void Disconnect() {
            DisconnectInner(DisconnectionType.LocalDisconnection);
        }



        private void ReceiveBegin() {
            try {
                // Request next chunk
                ClientStream.BeginRead(ReceiveBuffer, ReceiveCallback, null);
            } catch (ObjectDisposedException) {
            } catch (IOException) {
                // Report connection failure
                DisconnectInner(DisconnectionType.ConnectionInterupted);
            }
        }

        private void ReceiveCallback(IAsyncResult ar) {
            // Complete receive and get read length
            int rxBytes = 0;
            try {
                rxBytes = ClientStream.EndRead(ar);
            } catch (ObjectDisposedException) {
                return;
            } catch (IOException) {
                DisconnectInner(DisconnectionType.ConnectionInterupted);
                return;
            }

            // Handle connection reset
            if (rxBytes == 0) {
                DisconnectInner(DisconnectionType.RemoteDisconnection);
                return;
            }

            // Update buffer count
            ReceiveBuffer.IncrementEnd(rxBytes);

            // Increment received bytes
            TotalRxBytes = unchecked(TotalRxBytes + rxBytes);

            // TODO: Handle keep-alive 0 bytes !!!!!!!!!!!!!!!!!!!!!!!!!!!

            // Attempt to get length
            var length = NextPayload.GetPayloadLength(ReceiveBuffer);

            throw new NotImplementedException();

            /*
            // Alert new bytes available
            OnDataArrived(ReceiveBuffer);

            // If data has been consumed, reset buffer size
            if (ReceiveBuffer.Start > 0) {
                ReceiveBuffer = ReceiveBuffer.Resize(Options.DecodingBufferInitialSize); ///////////////
            } else if (ReceiveBuffer.Available == 0) { // If buffer is full
                // Check if max size has been reached
                if (ReceiveBuffer.Count >= Options.PayloadMaxSize) {
                    DisconnectInner(DisconnectionType.ReceiveBufferFull);
                    return;
                }

                // Grow buffer
                ReceiveBuffer = ReceiveBuffer.Resize((int)(ReceiveBuffer.Count * Options.DecodingBufferGrowthRate));
            }
            */
            // Get next chunk
            ReceiveBegin();
        }

        /// <summary>
        /// Fires when a payload hasn't been sent in the keep-alive interval in order to prevent a receive-timeout on the remote end.
        /// </summary>
        private void KeepAliveTimer_OnElapsed(object sender, System.Timers.ElapsedEventArgs e) {
            // Send blank payload - it will reset the timeout on the remote end, however not be delivered as an actual payload
            RawSend(new Buffer<byte>(new byte[] { 0 }), null);
        }

        /// <summary>
        /// Handle internal disconnect requests.
        /// </summary>
        /// <param name="reason"></param>
        private void DisconnectInner(DisconnectionType reason) {
            if (IsDisposed) {
                return;
            }
            Dispose();

            OnDisconnected.TryInvoke(reason);
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Stop keep-alive sending
                KeepAliveTimer.StopIfNotNull();

                // Dispose managed state (managed objects)
                ClientStream.DisposeIfNotNull();

                var clientSocket = ClientSocket;
                if (null != clientSocket) {
                    try {
                        // Kill socket (being nice about it)
                        clientSocket.Close();
                    } catch { }

                    // Dispose socket
                    clientSocket.Dispose();
                }

                KeepAliveTimer.DisposeIfNotNull();
            }

            // Set large fields to null
            //ClientSocket = null;
            //ClientStream = null; // Do not set to null
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }


        /// <summary>
        /// Validate certificates given by servers, on the client end.
        /// </summary>
        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors) {
            // If there are no errors, return success
            if (policyErrors == SslPolicyErrors.None) {
                return true;
            }

            // Do not allow this client to communicate with unauthenticated servers
            return false;
        }
    }
}
