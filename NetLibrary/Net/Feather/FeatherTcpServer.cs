using System;
using System.Collections.Generic;
using System.Text;

namespace InvertedTomato.Net.Feather {
    public class FeatherTcpServer {
        private readonly Socket Underlying = new Socket(SocketType.Stream, ProtocolType.Tcp);
        private readonly Object Sync = new Object();

        public event Action<EndPoint, TMessage> OnMessageReceived;
        public event Action<EndPoint> OnClientConnected;
        public event Action<EndPoint, DisconnectionType> OnClientDisconnected;

        public bool IsDisposed { get; private set; }

        public void Listen(UInt32 port) { // Tcp, Udp
            throw new NotImplementedException();
        }
        public void Disconnect(EndPoint address) { // Tcp-server, TcpSsl-server
            throw new NotImplementedException();
        }





        public void SendTo(EndPoint address, TMessage msg) { // Tcp, TcpSsl, Udp
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Dispose managed state (managed objects)
                Underlying.Dispose();
            }
        }

        public void Dispose() {
            Dispose(true);
        }
    }
}
