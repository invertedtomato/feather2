using InvertedTomato.IO.Messages;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace InvertedTomato.Net.Feather {
    public class FeatherTcpClient<TMessage> : IDisposable where TMessage : IImportableMessage, IExportableMessage, new() {
        private readonly Socket Underlying = new Socket(SocketType.Stream, ProtocolType.Tcp);
        private readonly Object Sync = new Object();

        public bool IsDisposed { get; private set; }
        public bool NoDelay { get { return Underlying.NoDelay; } set { Underlying.NoDelay = value; } }

        public event Action<TMessage> OnMessageReceived;
        public event Action OnPokeReceived;
        public event Action<DisconnectionType> OnDisconnected;

        public void Connect(string host, int port) {
            Underlying.Connect(host, port);
        }
        public void Connect(EndPoint address) {
            Underlying.Connect(address);
        }
        public async Task ConnectAsync(string host, int port) {
            await Underlying.ConnectAsync(host, port);
        }
        public async Task ConnectAsync(EndPoint address) {
            await Underlying.ConnectAsync(address);
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

        public async Task SendAsync(TMessage message) {
            throw new NotImplementedException();
        }

        public void Poke() {
            throw new NotImplementedException();
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
    }
}
