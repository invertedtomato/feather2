using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using InvertedTomato.IO.Messages;

// TODO: Async
namespace InvertedTomato.Net.Feather {
    public class FeatherUdpPeer<TMessage> : IDisposable where TMessage : IImportableMessage, IExportableMessage, new() {
        private readonly Socket Underlying = new Socket(SocketType.Dgram, ProtocolType.Udp);
        private readonly Object Sync = new Object();

        public event Action<EndPoint, TMessage> OnMessageReceived;

        public Int32 ReceiveBufferSize { get { return Underlying.ReceiveBufferSize; } set { Underlying.ReceiveBufferSize = value; } }
        public Int32 ReceiveMaxMessageSize { get; set; } = 1500;

        public bool IsDisposed { get; private set; }

        public FeatherUdpPeer() {
            
        }

        private void StartReceive() {
            var args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            args.SocketFlags = SocketFlags.None;
            args.SetBuffer(new byte[ReceiveMaxMessageSize], 0, ReceiveMaxMessageSize);
            args.Completed += (sender, e) => {
                // Instantiate message
                var message = new TMessage();
                message.Import(new ArraySegment<byte>(e.Buffer, 0, e.BytesTransferred));

                // Raise received event
                OnMessageReceived(e.RemoteEndPoint, message);

                StartReceive();
            };

            try {
                Underlying.ReceiveMessageFromAsync(args);
            } catch (ObjectDisposedException) { };
        }

        public void Bind(Int32 port) {
            Bind(new IPEndPoint(IPAddress.Any, port));
        }
        public void Bind(EndPoint endPoint) {
            lock (Sync) {
                // Bind underlying socket
                Underlying.Bind(endPoint);

                // Start receiving
                StartReceive();
            }
        }

        public async Task SendTo(EndPoint target, TMessage message) {
            if (null == message) {
                throw new ArgumentNullException(nameof(message));
            }

            // Extract payload from message
            var payload = message.Export();

            // Send!
            await Underlying.SendToAsync(payload, SocketFlags.None, target);
        }

        public void SendToSync(EndPoint target, TMessage message) {
            if (null == message) {
                throw new ArgumentNullException(nameof(message));
            }

            // Extract payload from message
            var payload = message.Export();

            // Send!
            Underlying.SendTo(payload.Array, payload.Offset, payload.Count, SocketFlags.None, target);
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


        /*
        
        //public event Action<EndPoint> OnClientConnected;
        //public event Action<EndPoint, DisconnectionType> OnClientDisconnected;

        public void Listen(UInt32 port) { // Tcp, Udp
            throw new NotImplementedException();
        }
        public void ListenSecure(UInt32 port, X509Certificate certificate) { // TcpSsl

        }
        public void Unlisten() { // Tcp, TcpSsl, Udp

        }
        public void Disconnect(EndPoint address) { // Tcp-server, TcpSsl-server
            throw new NotImplementedException();
        }


        public void Connect(EndPoint address) { // Tcp
            throw new NotImplementedException();
        }
        public void ConnectSecure(EndPoint address) { // TcpSsl

        }
        public void Disconnect() { // Tcp-client, TcpSsl-client

        }


        public void SendTo(? address, TMessage msg) { // Tcp, TcpSsl, Udp
            throw new NotImplementedException();
        }

        public void Send(TMessage msg) { // Tcp-client, TcpSsl-client
            throw new NotImplementedException();
        }*/
    }
}