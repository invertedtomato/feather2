using InvertedTomato.IO.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace InvertedTomato.Net.Feather {
    public class FeatherTcpServer<TMessage> : IDisposable where TMessage : IImportableMessage, IExportableMessage, new() {
        private readonly Socket Underlying = new Socket(SocketType.Stream, ProtocolType.Tcp);
        private readonly ConcurrentDictionary<EndPoint, Client> Clients = new ConcurrentDictionary<EndPoint, Client>();
        private readonly Object Sync = new Object();

        public event Action<EndPoint, TMessage> OnMessageReceived;
        public event Action<EndPoint> OnClientConnected;
        public event Action<EndPoint, DisconnectionType> OnClientDisconnected;

        public bool IsDisposed { get; private set; }

        public void Listen(Int32 port) {
            Listen(new IPEndPoint(IPAddress.Any, port));
        }
        public void Listen(IPEndPoint localEndPoint) {
            lock (Sync) {
                Underlying.Bind(localEndPoint);
                Underlying.Listen(100);

                Accept();
            }
        }
        public void Disconnect(EndPoint address) {
            throw new NotImplementedException();
        }





        public void SendTo(EndPoint address, TMessage msg) {
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


        private void Accept() {
            var args = new SocketAsyncEventArgs();
            args.Completed += (sender, e) => {
                var client = Clients[e.RemoteEndPoint] = new Client() {
                    Socket = e.AcceptSocket,
                    LengthBuffer = new Byte[2],
                    LengthCount = 0
                };

                ReceiveLength(client);

                Accept();
            };
            Underlying.AcceptAsync(args);
        }

        private void ReceiveLength(Client client) {
            var args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            args.SocketFlags = SocketFlags.None;
            args.SetBuffer(client.LengthBuffer, client.LengthCount, 2 - client.LengthCount);
            args.Completed += (sender, e) => {
                // Update byte received count with what was just received
                client.LengthCount += e.BytesTransferred;

                if (client.LengthCount < 2) {
                    // Not all length received - get more
                    ReceiveLength(client);
                } else {
                    // Compute length
                    var length = BitConverter.ToUInt16(client.LengthBuffer, 0);
                    client.PayloadBuffer = new byte[length];

                    // Reset state
                    client.LengthCount = 0;

                    // Receive payload now
                    ReceivePayload(client);
                }

            };

            try {
                client.Socket.ReceiveAsync(args);
            } catch (ObjectDisposedException) { };
        }

        private void ReceivePayload(Client client) {
            var args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            args.SocketFlags = SocketFlags.None;
            args.SetBuffer(client.PayloadBuffer, client.PayloadCount, client.PayloadBuffer.Length - client.PayloadCount);
            args.Completed += (sender, e) => {
                if (e.BytesTransferred < 0) {
                    throw new Exception(); // TODO
                }

                // Update received count
                client.PayloadCount += e.BytesTransferred;

                if (client.PayloadCount < client.PayloadBuffer.Length) {
                    ReceivePayload(client);
                } else {
                    // Instantiate message
                    var message = new TMessage();
                    message.Import(new ArraySegment<byte>(client.PayloadBuffer, 0, client.PayloadBuffer.Length));

                    // Raise received event
                    OnMessageReceived(e.RemoteEndPoint, message);

                    ReceiveLength(client);
                }
            };

            try {
                client.Socket.ReceiveAsync(args);
            } catch (ObjectDisposedException) { };
        }
    }

    class Client {
        public Socket Socket;

        public Byte[] LengthBuffer;
        public Int32 LengthCount;

        public Byte[] PayloadBuffer;
        public Int32 PayloadCount;

    }
}
