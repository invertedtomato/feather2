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
    public sealed class Remote<TEncoder, TDecoder> : IDisposable
        where TEncoder : IEncoder
        where TDecoder : IDecoder, new() {

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
        /// When the remote disconnects.
        /// </summary>
        public Action<DisconnectionType> OnDisconnection;

        /// <summary>
        /// When a message arrives from this remote.
        /// </summary>
        public Action<TDecoder> OnMessage;

        /// <summary>
        /// Configuration options
        /// </summary>
        private  ConnectionOptions Options;

        /// <summary>
        /// Timer to send keep-alive payloads to prevent disconnection.
        /// </summary>
        private  System.Timers.Timer KeepAliveTimer;

        /// <summary>
        /// Client socket.
        /// </summary>
        private  ISocket ClientSocket;

        /// <summary>
        /// Client stream.
        /// </summary>
        private  IStream ClientStream;

        /// <summary>
        /// Receive buffers.
        /// </summary>
        private  Buffer<byte> HeaderBuffer;

        /// <summary>
        /// Number of payloads that haven't made it to the TCP buffer yet. Will be used for back-pressure indication.
        /// </summary>
        private int OutstandingSends = 0;

        private Buffer<byte> PayloadBuffer = null;

        private TDecoder NextMessage;

        public void Start(bool isServerConnection, ISocket clientSocket, ConnectionOptions options, Action<DisconnectionType> onDisconnection, Action<TDecoder> onMessage) {
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

            // Store
            Options = options;
            OnDisconnection = onDisconnection;
            OnMessage = onMessage;

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
            if (options.UseApplicationLayerKeepAlive) {
                // Start keep-alive timer (must be before receive start)
                KeepAliveTimer = new System.Timers.Timer(options.KeepAliveInterval.TotalMilliseconds);
                KeepAliveTimer.Elapsed += KeepAliveTimer_OnElapsed;
                KeepAliveTimer.Start();
            } else {
                // Enable TCP keep-alive
                ClientSocket.SetKeepAlive(true, options.KeepAliveInterval);
            }

            // Setup receive
            NextMessage = new TDecoder();
            HeaderBuffer = new Buffer<byte>(NextMessage.MaxHeaderLength);

            // Seed receiving
            ReceiveHeaderChunk();
        }

        /// <summary>
        /// Send single payload to remote endpoint.
        /// </summary>    
        public void Send(TEncoder message) {
#if DEBUG
            if (null == message) {
                throw new ArgumentNullException("payload");
            }
#endif

            RawSend(message.GetBuffer(), null);
        }

        /// <summary>
        /// Send single payload to remote endpoint and execute a callback when done.
        /// </summary>
        public void Send(TEncoder message, Action done) {
#if DEBUG
            if (null == message) {
                throw new ArgumentNullException("payload");
            }
#endif

            RawSend(message.GetBuffer(), done);
        }

        /// <summary>
        /// Send multiple payloads to remote endpoint.
        /// </summary>    
        public void Send(TEncoder[] messages) {
#if DEBUG
            if (null == messages) {
                throw new ArgumentNullException("payload");
            }
#endif

            Send(messages, null);
        }

        /// <summary>
        /// Send multiple payloads to remote endpoint and execute a callback when done.
        /// </summary>
        public void Send(TEncoder[] messages, Action done) {
#if DEBUG
            if (null == messages) {
                throw new ArgumentNullException("messages");
            }
            foreach (var message in messages) {
                if (null == message) {
                    throw new ArgumentNullException("messages", "Element in array.");
                }
            }
#endif

            // Get buffers
            var payloadBuffers = messages.Select(a => a.GetBuffer());

            // Merge into master buffer
            var masterLength = payloadBuffers.Sum(a => a.Used);
            var masterBuffer = new Buffer<byte>(masterLength);
            payloadBuffers.Each(a => masterBuffer.EnqueueBuffer(a));

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
            if (Options.UseApplicationLayerKeepAlive) {
                KeepAliveTimer.Restart();
            }
        }

        /// <summary>
        /// Disconnect from the remote endpoint and dispose.
        /// </summary>
        public void Disconnect() {
            DisconnectInner(DisconnectionType.LocalDisconnection);
        }

        private void ReceiveHeaderChunk() {
            // Calculate size of next chunk to receive
            var chunkSize = HeaderBuffer.IsEmpty ? NextMessage.MinHeaderLength : 1;

            try {
                // Request next chunk
                ClientStream.BeginRead(HeaderBuffer, chunkSize, OnHeaderChunkReceived, null);
            } catch (ObjectDisposedException) {
            } catch (IOException) {
                // Report connection failure
                DisconnectInner(DisconnectionType.ConnectionInterupted);
            }
        }

        private void OnHeaderChunkReceived(IAsyncResult ar) {
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

            // Update buffer end
            HeaderBuffer.IncrementEnd(rxBytes);

            // Increment received bytes
            TotalRxBytes = unchecked(TotalRxBytes + rxBytes);

            // Attempt to get length
            var length = NextMessage.GetPayloadLength(HeaderBuffer);

            // If length is unknown...
            if (length == 0) {
                // Check if the header is too long
                if (HeaderBuffer.IsFull) {
                    DisconnectInner(DisconnectionType.MalformedPayload);
                    return;
                }

                // Get another byte
                ReceiveHeaderChunk();
                return;
            }

            // Allocate payload buffer
            PayloadBuffer = HeaderBuffer.Resize(length);

            // Clear header buffer
            HeaderBuffer.Reset();

            // Receive payload
            ReceivePayload();
        }

        private void ReceivePayload() {
            // If not all payload has arrived...
            if (!PayloadBuffer.IsFull) {
                // Fetch another chunk
                ReceivePayloadChunk();

                return;
            }

            // Prepare message
            NextMessage.LoadBuffer(PayloadBuffer);

            // Return message
            OnMessage.TryInvoke(NextMessage);

            // Reset for next message
            NextMessage = new TDecoder();

            // Receive next header
            ReceiveHeaderChunk();
        }

        private void ReceivePayloadChunk() {
            try {
                // Request next chunk
                ClientStream.BeginRead(PayloadBuffer, OnPayloadChunkReceived, null);
            } catch (ObjectDisposedException) {
            } catch (IOException) {
                // Report connection failure
                DisconnectInner(DisconnectionType.ConnectionInterupted);
            }
        }

        private void OnPayloadChunkReceived(IAsyncResult ar) {
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

            // Update buffer end
            PayloadBuffer.IncrementEnd(rxBytes);

            // Receive payload
            ReceivePayload();
        }

        /// <summary>
        /// Fires when a payload hasn't been sent in the keep-alive interval in order to prevent a receive-timeout on the remote end.
        /// </summary>
        private void KeepAliveTimer_OnElapsed(object sender, System.Timers.ElapsedEventArgs e) {
            // Send blank payload - it will reset the timeout on the remote end, however not be delivered as an actual payload
            RawSend(NextMessage.GetNullPayload(), null);
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

            OnDisconnection.TryInvoke(reason);
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing) {
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
