using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Feather;
using InvertedTomato.Testable;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace InvertedTomato.Net.Feather {
    public sealed class FeatherServer<TMessage> : IDisposable
        where TMessage : IMessage, new() {

        /// <summary>
        /// When a client connects.
        /// </summary>
        public event Action<EndPoint, FeatherClient<TMessage>> OnClientConnection;

        /// <summary>
        /// When a client disconnects.
        /// </summary>
        public event Action<EndPoint, FeatherClient<TMessage>, DisconnectionType> OnClientDisconnection;

        /// <summary>
        /// When a inbound message arrives
        /// </summary>
        public event Action<EndPoint, FeatherClient<TMessage>, TMessage> OnMessageReceived;

        /// <summary>
        /// Has the server been disposed.
        /// </summary>
        public Boolean IsDisposed { get; private set; }

        /// <summary>
        /// User provided options.
        /// </summary>
        private readonly Options Options;

        /// <summary>
        /// Socket the server is listening on.
        /// </summary>
        private Socket ListenerSocket = null;

        /// <summary>
        /// All active remotes (may contain some very recently disconnected.)
        /// </summary>
        private ConcurrentDictionary<EndPoint, FeatherClient<TMessage>> Remotes = new ConcurrentDictionary<EndPoint, FeatherClient<TMessage>>();


        /// <summary>
        /// Simple instantiation.
        /// </summary>
        public FeatherServer(Int32 port) : this(new IPEndPoint(IPAddress.Any, port), new Options()) { }

        /// <summary>
        /// Instantiate with options
        /// </summary>
        /// <param name="options"></param>
        public FeatherServer(EndPoint endpoint, Options options) {
            if(null == endpoint) {
                throw new ArgumentNullException(nameof(endpoint));
            }
            if(null == options) {
                throw new ArgumentNullException(nameof(options));
            }

            // Store options
            Options = options;

            // Open socket
            ListenerSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            ListenerSocket.Bind(endpoint);
            ListenerSocket.Listen(Options.MaxListenBacklog);

            // Seed accepting
            AcceptBegin();
        }

        public void SendToAll(TMessage message) {
#if DEBUG
            if(null == message) {
                throw new ArgumentNullException("payload");
            }
#endif

            SendToAll(new TMessage[] { message });
        }

        public void SendToAll(TMessage[] messages) {
#if DEBUG
            if(null == messages) {
                throw new ArgumentNullException("payload");
            }
#endif

            // Send to all remotes
            foreach(var remote in Remotes) {
                remote.Send(messages);
            }
        }


        private void AcceptBegin() {
            // Wait for, and accept next connection
            ListenerSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private void AcceptCallback(IAsyncResult ar) {
            try {
                // Get client socket
                var clientSocket = ListenerSocket.EndAccept(ar);
                var endpoint = clientSocket.RemoteEndPoint;

                // Create remote
                var remote = Remotes[endpoint] = new FeatherClient();
                remote.Start(true, new SocketReal(clientSocket), Options,
                    (reason) => {
                        OnClientDisconnection(endpoint, remote, reason);
                        Remotes.TryRemove(endpoint);
                    },
                    (message) => { OnMessageReceived(remote, message); }
                );

                // Raise event
                OnClientConnection(endpoint, remote);

                // Resume accepting sockets
                AcceptBegin();
            } catch(ObjectDisposedException) { } // This occurs naturally during dispose
        }

        public void Dispose() { Dispose(true); }
        void Dispose(Boolean disposing) {
            if(IsDisposed) {
                return;
            }
            IsDisposed = true;

            if(disposing) {
                // Dispose managed state (managed objects)
                ListenerSocket.Dispose();
            }
        }
    }
}
