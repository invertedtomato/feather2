using InvertedTomato.Testable.Sockets;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace InvertedTomato.Net.Feather {
    public sealed class FeatherTCP<TCodec> : IDisposable
        where TCodec : IIntegerCodec, new() {

        /// <summary>
        /// When a client connects.
        /// </summary>
        public Action<Remote<TEncoder, TDecoder>> OnConnection;

        /// <summary>
        /// When a client disconnects.
        /// </summary>
        public Action<Remote<TEncoder, TDecoder>, DisconnectionType> OnDisconnection;

        /// <summary>
        /// When a inbound message arrives
        /// </summary>
        public Action<Remote<TEncoder, TDecoder>, TDecoder> OnMessage;

        /// <summary>
        /// Has the server been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// User provided options.
        /// </summary>
        private readonly ConnectionOptions Options;

        /// <summary>
        /// Socket the server is listening on.
        /// </summary>
        private Socket ListenerSocket = null;

        /// <summary>
        /// All active remotes (may contain some very recently disconnected.)
        /// </summary>
        private ConcurrentDictionary<EndPoint, Remote<TEncoder, TDecoder>> Remotes = new ConcurrentDictionary<EndPoint, Remote<TEncoder, TDecoder>>();


        /// <summary>
        /// Simple instantiation.
        /// </summary>
        public FeatherTCP() : this(new ConnectionOptions()) { }

        /// <summary>
        /// Instantiate with options
        /// </summary>
        /// <param name="options"></param>
        public FeatherTCP(ConnectionOptions options) {
            if (null == options) {
                throw new ArgumentNullException("options");
            }

            // Store options
            Options = options;
        }


        /// <summary>
        /// Start a Feather server by listening for connections.
        /// </summary>
        /// <returns>Feather instance</returns>
        public void Listen(int port) {
            Listen(new IPEndPoint(IPAddress.Any, port));
        }

        /// <summary>
        /// Start a Feather server by listening for connections.
        /// </summary>
        /// <returns>Feather instance</returns>
        public void Listen(EndPoint endPoint) {
            if (null == endPoint) {
                throw new ArgumentNullException("endpoint");
            }

            try {
                // Open socket
                ListenerSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                ListenerSocket.Bind(endPoint);
                ListenerSocket.Listen(Options.MaxListenBacklog);

                // Seed accepting
                AcceptBegin();
            } catch (ObjectDisposedException) { } // This occurs if the server is disposed during instantiation
        }


        /// <summary>
        /// Connect to a Feather server.
        /// </summary>
        /// <returns>Server connection</returns>
        public Remote<TEncoder, TDecoder> Connect(IPAddress serverAddress, int port) { return Connect(new IPEndPoint(serverAddress, port)); }

        /// <summary>
        /// Connect to a Feather server.
        /// </summary>
        /// <returns>Server connection</returns>
        public Remote<TEncoder, TDecoder> Connect(string serverName, int port) { return Connect(new DnsEndPoint(serverName, port)); }

        /// <summary>
        /// Connect to a Feather server.
        /// </summary>
        /// <returns>Server connection</returns>
        public Remote<TEncoder, TDecoder> Connect(EndPoint endPoint) {
#if DEBUG
            if (null == endPoint) {
                throw new ArgumentNullException("endPoint");
            }
#endif

            // Open socket
            var clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(endPoint);

            // Create remote
            var remote = Remotes[endPoint] = new Remote<TEncoder, TDecoder>();
            remote.Start(false, new SocketReal(clientSocket), Options,
                (reason) => {
                    OnDisconnection(remote, reason);
                    Remotes.TryRemove(endPoint);
                },
                (message) => { OnMessage(remote, message); }
            );

            return remote;
        }


        public void Broadcast(TEncoder message) {
#if DEBUG
            if (null == message) {
                throw new ArgumentNullException("payload");
            }
#endif

            Broadcast(new TEncoder[] { message });
        }

        public void Broadcast(TEncoder[] messages) {
#if DEBUG
            if (null == messages) {
                throw new ArgumentNullException("payload");
            }
#endif

            // Send to all remotes
            Remotes.Each(a => a.Value.Send(messages));
        }


        private void AcceptBegin() {
            // Wait for, and accept next connection
            ListenerSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private void AcceptCallback(IAsyncResult ar) {
            try {
                // Get client socket
                var clientSocket = ListenerSocket.EndAccept(ar);
                var endPoint = clientSocket.RemoteEndPoint;

                // Create remote
                var remote = Remotes[endPoint] = new Remote<TEncoder, TDecoder>();
                remote.Start(true, new SocketReal(clientSocket), Options,
                    (reason) => {
                        OnDisconnection(remote, reason);
                        Remotes.TryRemove(endPoint);
                    },
                    (message) => { OnMessage(remote, message); }
                );

                // Raise event
                OnConnection.TryInvoke(remote);

                // Resume accepting sockets
                AcceptBegin();
            } catch (ObjectDisposedException) { } // This occurs naturally during dispose
        }

        public void Dispose() { Dispose(true); }
        void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Dispose managed state (managed objects)
                ListenerSocket.Dispose();
            }
        }




    }
}
