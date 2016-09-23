using InvertedTomato.Testable.Sockets;
using System;
using System.Net;
using System.Net.Sockets;
using ThreePlay.IO.Feather;

namespace InvertedTomato.Net.Feather {
    public sealed class FeatherTCP<TConnection, TEncoder, TDecoder> : IDisposable 
        where TDecoder : IDecoder, new() 
        where TEncoder : IEncoder, new() 
        where TConnection : ConnectionBase<TEncoder, TDecoder>, new() {

        /// <summary>
        /// When a client connects.
        /// </summary>
        public Action<TConnection> OnClientConnected;

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
        private readonly Socket ListenerSocket;

        internal FeatherTCP(EndPoint endPoint, ConnectionOptions options) {
            // Store configuration
            Options = options;

            try {
                // Open socket
                ListenerSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                ListenerSocket.Bind(endPoint);
                ListenerSocket.Listen(Options.MaxListenBacklog);

                // Seed accepting
                AcceptBegin();
            } catch (ObjectDisposedException) { } // This occurs if the server is disposed during instantiation
        }

        private void AcceptBegin() {
            // Wait for, and accept next connection
            ListenerSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private void AcceptCallback(IAsyncResult ar) {
            try {
                // Get client socket
                var clientSocket = ListenerSocket.EndAccept(ar);

                // Create connection
                var connection = new TConnection();
                connection.Start(true, new SocketReal(clientSocket), Options);

                // Raise event
                OnClientConnected.TryInvoke(connection);

                // Resume accepting sockets
                AcceptBegin();
            } catch (ObjectDisposedException) { } // This occurs naturally during dispose
        }

        /*
         public void Broadcast(object topic, Message message) {}
         */

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


        /// <summary>
        /// Start a Feather server by listening for connections.
        /// </summary>
        /// <returns>Feather instance</returns>
        public static FeatherTCP<TConnection, TEncoder, TDecoder> Listen(int port) { return Listen(new IPEndPoint(IPAddress.Any, port)); }

        /// <summary>
        /// Start a Feather server by listening for connections.
        /// </summary>
        /// <returns>Feather instance</returns>
        public static FeatherTCP<TConnection, TEncoder, TDecoder> Listen(int port, ConnectionOptions options) { return Listen(new IPEndPoint(IPAddress.Any, port), options); }

        /// <summary>
        /// Start a Feather server by listening for connections.
        /// </summary>
        /// <returns>Feather instance</returns>
        public static FeatherTCP<TConnection, TEncoder, TDecoder> Listen(EndPoint localEndPoint) { return Listen(localEndPoint, new ConnectionOptions()); }

        /// <summary>
        /// Start a Feather server by listening for connections.
        /// </summary>
        /// <returns>Feather instance</returns>
        public static FeatherTCP<TConnection, TEncoder, TDecoder> Listen(EndPoint localEndPoint, ConnectionOptions options) {
#if DEBUG
            if (null == localEndPoint) {
                throw new ArgumentNullException("endpoint");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            return new FeatherTCP<TConnection, TEncoder, TDecoder>(localEndPoint, options);
        }

        /// <summary>
        /// Connect to a Feather server.
        /// </summary>
        /// <returns>Server connection</returns>
        public static TConnection Connect(IPAddress serverAddress, int port) { return Connect(new IPEndPoint(serverAddress, port)); }

        /// <summary>
        /// Connect to a Feather server.
        /// </summary>
        /// <returns>Server connection</returns>
        public static TConnection Connect(IPAddress serverAddress, int port, ConnectionOptions options) { return Connect(new IPEndPoint(serverAddress, port), options); }

        /// <summary>
        /// Connect to a Feather server.
        /// </summary>
        /// <returns>Server connection</returns>
        public static TConnection Connect(string serverName, int port) { return Connect(new DnsEndPoint(serverName, port)); }

        /// <summary>
        /// Connect to a Feather server.
        /// </summary>
        /// <returns>Server connection</returns>
        public static TConnection Connect(string serverName, int port, ConnectionOptions options) { return Connect(new DnsEndPoint(serverName, port), options); }

        /// <summary>
        /// Connect to a Feather server.
        /// </summary>
        /// <returns>Server connection</returns>
        public static TConnection Connect(EndPoint endPoint) { return Connect(endPoint, new ConnectionOptions()); }

        /// <summary>
        /// Connect to a Feather server.
        /// </summary>
        /// <returns>Server connection</returns>
        public static TConnection Connect(EndPoint endPoint, ConnectionOptions options) {
#if DEBUG
            if (null == endPoint) {
                throw new ArgumentNullException("endPoint");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Open socket
            var clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(endPoint);

            // Create connection
            var connection = new TConnection();
            connection.Start(false, new SocketReal(clientSocket), options);

            return connection;
        }
    }
}
