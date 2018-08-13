using InvertedTomato.IO.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InvertedTomato.Net.Feather {
    public class FeatherTcpClient<TMessage> : IDisposable where TMessage : IImportableMessage, IExportableMessage, new() {
        private static readonly Byte[] BlankPayload = new Byte[] { 0, 0 };
        private readonly Socket Underlying = new Socket(SocketType.Stream, ProtocolType.Tcp);
        private Stream UnderlyingStream;
        private readonly Object Sync = new Object();

        private Byte[] LengthBuffer = new byte[2];
        private Int32 LengthCount;

        private Byte[] PayloadBuffer;
        private Int32 PayloadCount;

        public bool IsDisposed { get; private set; }
        public bool NoDelay { get { return Underlying.NoDelay; } set { Underlying.NoDelay = value; } }

        public event Action<TMessage> OnMessageReceived;
        private event Action OnPokeReceived;
        public event Action<DisconnectionType> OnDisconnected;

        public void Connect (string host, int port) {
            // Connect
            Underlying.Connect(host, port);
            UnderlyingStream = new NetworkStream(Underlying, true);

            // Seed receive process
            Task.Run((Action)ReceiveLength);
        }

        public async Task ConnectAsync (string host, int port) {
            // Connect
            await Underlying.ConnectAsync(host, port);
            UnderlyingStream = new NetworkStream(Underlying, true);

            // Seed receive process
            var receiveTask = Task.Run((Action)ReceiveLength);
        }

        public async Task ConnectSecureAsync (string host, int port) {
            await ConnectSecureAsync(host, port, host, null);
        }

        public async Task ConnectSecureAsync (string host, int port, string validateHost, RemoteCertificateValidationCallback remoteCertificateValidationCallback) {
            // Connect
            await Underlying.ConnectAsync(host, port);

            // Create secure stream and authenticate
            var secureStream = new SslStream(new NetworkStream(Underlying, true), false, remoteCertificateValidationCallback); // Wrap the network stream in a SslStream
            await secureStream.AuthenticateAsClientAsync(validateHost);
            UnderlyingStream = secureStream;

            // Seed receive process
            ReceiveLength();
        }


        public void Send (TMessage message) {
            if (null == message) {
                throw new ArgumentNullException(nameof(message));
            }

            // Extract payload from message
            var payload = message.Export();

            // Check the payload is not too large
            if (payload.Count > UInt16.MaxValue) {
                throw new ArgumentOutOfRangeException(nameof(message), "Message must encode to a payload of 65KB or less");
            }

            // Convert length to header
            var lengthBytes = BitConverter.GetBytes((UInt16)payload.Count);

            // Send length header, followed by payload
            var stream = UnderlyingStream;
            if (null == stream) {
                throw new InvalidOperationException();
            }
            stream.Write(lengthBytes);
            stream.Write(payload);
        }

        public async Task SendAsync (TMessage message) {
            if (null == message) {
                throw new ArgumentNullException(nameof(message));
            }

            // Extract payload from message
            var payload = message.Export();

            // Check the payload is not too large
            if (payload.Count > UInt16.MaxValue) {
                throw new ArgumentOutOfRangeException(nameof(message), "Message must encode to a payload of 65KB or less");
            }

            // Convert length to header
            var lengthBytes = BitConverter.GetBytes((UInt16)payload.Count);

            // Send length header, followed by payload
            var stream = UnderlyingStream;
            if (null == stream) {
                throw new InvalidOperationException();
            }
            await stream.WriteAsync(lengthBytes);
            await stream.WriteAsync(payload);
        }

        private void Poke () {
            UnderlyingStream.Write(BlankPayload);
        }

        protected virtual void Dispose (bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Dispose managed state (managed objects)
                try {
                    Underlying.Shutdown(SocketShutdown.Both);
                } catch (Exception) { }

                UnderlyingStream?.Dispose();
                Underlying.Dispose();
            }
        }

        public void Dispose () {
            Dispose(true);
        }



        private void HandleRemoteDisconenct () {
            // Cleanup
            UnderlyingStream.Dispose();
            Underlying.Dispose();

            // Fire event handler
            OnDisconnected?.Invoke(DisconnectionType.RemoteDisconnection);
        }

        private async void ReceiveLength () {
            try {
                // Start the read
                var bytesTransfered = await UnderlyingStream.ReadAsync(LengthBuffer, LengthCount, LengthBuffer.Length - LengthCount);

                // Detect closed connection and handle
                if (bytesTransfered <= 0) {
                    HandleRemoteDisconenct();
                    return;
                }

                // Update byte received count with what was just received
                LengthCount += bytesTransfered;

                if (LengthCount < 2) {
                    // Not all length received - get more
                    ReceiveLength();
                } else {
                    // Compute length
                    var length = BitConverter.ToUInt16(LengthBuffer, 0);

                    // Abort if keep-alive message
                    if (length == 0) {
                        OnPokeReceived?.Invoke();
                        ReceiveLength();
                    }

                    // Allocate payload buffer
                    PayloadBuffer = new byte[length];

                    // Reset state
                    LengthCount = 0;

                    // Receive payload now
                    ReceivePayload();
                }
            } catch (ObjectDisposedException) {
            } catch (IOException) {
                HandleRemoteDisconenct();
            }
        }

        private async void ReceivePayload () {
            try {
                // Start the read
                var bytesTransfered = await UnderlyingStream.ReadAsync(PayloadBuffer, PayloadCount, PayloadBuffer.Length - PayloadCount);

                // Detect closed connection and handle
                if (bytesTransfered <= 0) {
                    HandleRemoteDisconenct();
                    return;
                }

                // Update received count
                PayloadCount += bytesTransfered;

                if (PayloadCount < PayloadBuffer.Length) {
                    // Not all payload received - get more
                    ReceivePayload();
                } else {
                    // Instantiate message
                    var message = new TMessage();
                    message.Import(new ArraySegment<byte>(PayloadBuffer, 0, PayloadBuffer.Length));

                    // Reset state
                    PayloadCount = 0;

                    // Raise received event
                    OnMessageReceived?.Invoke(message);

                    // Restart receive process with next lenght header
                    ReceiveLength();
                }
            } catch (ObjectDisposedException) {
            } catch (IOException) {
                HandleRemoteDisconenct();
            }
        }

        public Task ConnectSecureAsync(string v1, int v2, object test) {
            throw new NotImplementedException();
        }
    }
}
