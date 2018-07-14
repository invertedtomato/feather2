using InvertedTomato.IO.Messages;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InvertedTomato.Net.Feather {
    public class FeatherTcpClient<TMessage> : IDisposable where TMessage : IImportableMessage, IExportableMessage, new() {
        private readonly Socket Underlying = new Socket(SocketType.Stream, ProtocolType.Tcp);
        private readonly Object Sync = new Object();

        private Byte[] LengthBuffer = new byte[2];
        private Int32 LengthCount;

        private Byte[] PayloadBuffer;
        private Int32 PayloadCount;

        public bool IsDisposed { get; private set; }
        public bool NoDelay { get { return Underlying.NoDelay; } set { Underlying.NoDelay = value; } }

        public event Action<TMessage> OnMessageReceived;
        public event Action OnPokeReceived;
        public event Action<DisconnectionType> OnDisconnected;

        public void Connect(string host, int port) {
            Underlying.Connect(host, port);
            ReceiveLengthStart();
        }
        public void Connect(EndPoint address) {
            Underlying.Connect(address);
            ReceiveLengthStart();
        }
        public async Task ConnectAsync(string host, int port) {
            await Underlying.ConnectAsync(host, port);
            ReceiveLengthStart();
        }
        public async Task ConnectAsync(EndPoint address) {
            await Underlying.ConnectAsync(address);
            ReceiveLengthStart();
        }



        public void Send(TMessage message) {
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

            lock (Sync) {
                // Send length header, followed by payload
                Underlying.Send(lengthBytes);
                Underlying.Send(payload);
            }
        }

        public Task SendAsync(TMessage message) {
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

            // Coallese into one buffer
            var buffer = new byte[lengthBytes.Length + payload.Count];
            Buffer.BlockCopy(lengthBytes, 0, buffer, 0, 2);
            Buffer.BlockCopy(payload.Array, payload.Offset, buffer, 2, payload.Count);

            // Run send async
            return Task.Run(() => {
                lock (Sync) {
                    var block = new AutoResetEvent(false);

                    // Prepare async args
                    var args = new SocketAsyncEventArgs();
                    args.SetBuffer(buffer, 0, buffer.Length);
                    args.Completed += (sender, e) => {
                        block.Set();
                    };

                    // Send async - keeping in mind that it may return syncronously
                    var runningAsync = Underlying.SendAsync(args);
                    if (runningAsync) {
                        block.WaitOne();
                    }
                }
            });
        }

        public void Poke() {
            lock (Sync) {
                Underlying.Send(new byte[] { 0, 0 });
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Dispose managed state (managed objects)
                try {
                    Underlying.Shutdown(SocketShutdown.Both);
                } catch (Exception) { }

                Underlying.Dispose();
            }
        }

        public void Dispose() {
            Dispose(true);
        }



        private void Closed() {
            Underlying.Dispose();
            OnDisconnected?.Invoke(DisconnectionType.RemoteDisconnection);
        }

        private void ReceiveLengthStart() {
            try {
                // Prepare arguments
                var args = new SocketAsyncEventArgs();
                args.SetBuffer(LengthBuffer, LengthCount, 2 - LengthCount);
                args.Completed += (sender, e) => { ReceiveLenghtEnd(e); };

                // Start receiving length - note that this will not call Completed and return false if it completes synchronously
                if (!Underlying.ReceiveAsync(args)) {
                    Task.Run(() => { ReceiveLenghtEnd(args); });
                }
            } catch (ObjectDisposedException) { };
        }

        private void ReceiveLenghtEnd(SocketAsyncEventArgs args) {
            try {
                // Detect closed connection and handle
                if (args.BytesTransferred <= 0) {
                    Closed();
                    return;
                }

                // Update byte received count with what was just received
                LengthCount += args.BytesTransferred;

                if (LengthCount < 2) {
                    // Not all length received - get more
                    ReceiveLengthStart();
                } else {
                    // Compute length
                    var length = BitConverter.ToUInt16(LengthBuffer, 0);

                    // Abort if keep-alive message
                    if (length == 0) {
                        OnPokeReceived?.Invoke();
                        ReceiveLengthStart();
                    }

                    // Allocate payload buffer
                    PayloadBuffer = new byte[length];

                    // Reset state
                    LengthCount = 0;

                    // Receive payload now
                    ReceivePayloadStart();
                }
            } catch (ObjectDisposedException) { };
        }

        private void ReceivePayloadStart() {
            try {
                // Prepare arguments
                var args = new SocketAsyncEventArgs();
                args.SetBuffer(PayloadBuffer, PayloadCount, PayloadBuffer.Length - PayloadCount);
                args.Completed += (sender, e) => { ReceivePayloadEnd(e); };

                // Start receiving payload - note that this will not call Completed and return false if it completes synchronously
                if (!Underlying.ReceiveAsync(args)) {
                    Task.Run(() => { ReceivePayloadEnd(args); });
                }
            } catch (ObjectDisposedException) { };
        }

        private void ReceivePayloadEnd(SocketAsyncEventArgs args) {
            try {
                // Detect closed connection and handle
                if (args.BytesTransferred <= 0) {
                    Closed();
                    return;
                }

                // Update received count
                PayloadCount += args.BytesTransferred;

                if (PayloadCount < PayloadBuffer.Length) {
                    // Not all payload received - get more
                    ReceivePayloadStart();
                } else {
                    // Instantiate message
                    var message = new TMessage();
                    message.Import(new ArraySegment<byte>(PayloadBuffer, 0, PayloadBuffer.Length));

                    // Reset state
                    PayloadCount = 0;

                    // Raise received event
                    OnMessageReceived?.Invoke(message);

                    // Restart receive process with next lenght header
                    ReceiveLengthStart();
                }
            } catch (ObjectDisposedException) { };
        }

    }
}
