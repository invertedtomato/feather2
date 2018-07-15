using InvertedTomato.IO.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InvertedTomato.Net.Feather {
    public class FeatherTcpServer<TMessage> : IDisposable where TMessage : IImportableMessage, IExportableMessage, new() {
        private static readonly Byte[] BlankPayload = new Byte[] { 0, 0 };
        private readonly Socket Underlying = new Socket(SocketType.Stream, ProtocolType.Tcp);
        private readonly ConcurrentDictionary<EndPoint, Client> Clients = new ConcurrentDictionary<EndPoint, Client>();
        private readonly Object Sync = new Object();
        private X509Certificate Certificate = null;

        public event Action<EndPoint, TMessage> OnMessageReceived;
        private event Action<EndPoint> OnPokeReceived;
        public event Action<EndPoint> OnClientConnected;
        public event Action<EndPoint, DisconnectionType> OnClientDisconnected;

        /// <summary>
        /// Disable the Nagle algorithm so that packets are sent immediately. This sacrafices bandwidth savings for speed.
        /// </summary>
        public bool NoDelay { get { return Underlying.NoDelay; } set { Underlying.NoDelay = value; } }

        /// <summary>
        /// Is it disposed?
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// All remote endpoints currently connected.
        /// </summary>
        public IEnumerable<EndPoint> RemoteEndPoints { get { return Clients.Keys; } }

        /// <summary>
        /// Listen on a specified TCP port.
        /// </summary>
        /// <param name="port"></param>
        public void Listen(Int32 port) {
            Listen(new IPEndPoint(IPAddress.Any, port));
        }

        /// <summary>
        /// Listen on a specified local TCP endpoint.
        /// </summary>
        /// <param name="localEndPoint"></param>
        public void Listen(IPEndPoint localEndPoint) {
            lock (Sync) {
                Underlying.Bind(localEndPoint);
                Underlying.Listen(100);

                AcceptStart();
            }
        }

        /// <summary>
        /// Listen on a specified TCP port using SSL through the provided certificate.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="certificate"></param>
        public void ListenSecure(Int32 port, X509Certificate certificate) {
            if (null == certificate) {
                throw new ArgumentNullException(nameof(certificate));
            }

            Certificate = certificate;
            Listen(port);
        }

        /// <summary>
        /// Listen os a specfied local TCP endpoint using SSL through the provided certificate.
        /// </summary>
        /// <param name="localEndPoint"></param>
        /// <param name="certificate"></param>
        public void ListenSecure(IPEndPoint localEndPoint, X509Certificate certificate) {
            if (null == certificate) {
                throw new ArgumentNullException(nameof(certificate));
            }

            Certificate = certificate;
            Listen(localEndPoint);
        }

        /// <summary>
        /// Disconnect a remote endpoint. If endpoint is not connected no action is taken.
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        public void Disconnect(EndPoint remoteEndPoint) {
            if (null == remoteEndPoint) {
                throw new ArgumentNullException(nameof(remoteEndPoint));
            }

            if (!Clients.TryRemove(remoteEndPoint, out var client)) {
                return;
            }

            try {
                client.Socket.Shutdown(SocketShutdown.Both);
            } catch (SocketException) { };
            client.Stream.Dispose();
        }

        /// <summary>
        /// Send a message to a connected remote end point. No action is taken if the endpoint is not connected.
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        /// <param name="message"></param>
        public void SendTo(EndPoint remoteEndPoint, TMessage message) {
            if (null == remoteEndPoint) {
                throw new ArgumentNullException(nameof(remoteEndPoint));
            }
            if (null == message) {
                throw new ArgumentNullException(nameof(message));
            }

            // Get target client
            if (!Clients.TryGetValue(remoteEndPoint, out var client)) {
                return;
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
            client.Stream.Write(lengthBytes);
            client.Stream.Write(payload);
        }

        /// <summary>
        /// Send a message to a connected remote end point. No action is taken if the endpoint is not connected.
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        /// <param name="message"></param>
        public async Task SendToAsync(EndPoint remoteEndPoint, TMessage message) {
            if (null == remoteEndPoint) {
                throw new ArgumentNullException(nameof(remoteEndPoint));
            }
            if (null == message) {
                throw new ArgumentNullException(nameof(message));
            }

            // Get target client
            if (!Clients.TryGetValue(remoteEndPoint, out var client)) {
                return;
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
            await client.Stream.WriteAsync(lengthBytes);
            await client.Stream.WriteAsync(payload);
        }

        /// <summary>
        /// Send a blank message to confirm the remote endpoint is still alive. 
        /// </summary>
        /// <remarks>Failing to poke will mean that the remote might be gone, but we haven't yet realised. Sending a standard message has the same effect, though uses more data.</remarks>
        /// <param name="remoteEndPoint"></param>
        private void Poke(EndPoint remoteEndPoint) {
            if (null == remoteEndPoint) {
                throw new ArgumentNullException(nameof(remoteEndPoint));
            }

            // Get target client
            if (!Clients.TryGetValue(remoteEndPoint, out var client)) {
                return;
            }

            // Send length header, followed by payload
            client.Stream.Write(BlankPayload);
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Dispose managed state (managed objects)
                try {
                    Underlying.Shutdown(SocketShutdown.Both);
                } catch (SocketException) { }
                Underlying.Dispose();

                foreach (var item in Clients) {
                    try {
                        item.Value.Socket.Shutdown(SocketShutdown.Both);
                    } catch (Exception) { }
                    item.Value.Stream.Dispose();
                    item.Value.Socket.Dispose();
                }
            }
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }


        private void Closed(EndPoint endPoint) {
            if (Clients.TryRemove(endPoint, out var client)) {
                client.Stream.Dispose();
                client.Socket.Dispose();
                OnClientDisconnected?.Invoke(endPoint, DisconnectionType.RemoteDisconnection);
            }
        }

        private async Task AcceptStart() {
            try {
                // Prepare arguments
                var args = new SocketAsyncEventArgs();
                args.Completed += (sender, e) => { AcceptEnd(e); };

                // Start accept - note that this will not call Completed and return false if it completes synchronously
                if (!Underlying.AcceptAsync(args)) {
                    AcceptEnd(args);
                }
            } catch (ObjectDisposedException) { }
        }

        private async Task AcceptEnd(SocketAsyncEventArgs args) {
            try {
                // Get endpoint and socket
                var endPoint = args.AcceptSocket.RemoteEndPoint;
                var socket = args.AcceptSocket;

                // Facilitate shutdown
                if (null == endPoint) {
                    return;
                }

                // If secure, add SslStream layer
                var stream = (Stream)new NetworkStream(socket);
                if (null != Certificate) {
                    var secureStream = new SslStream(stream, false);
                    await secureStream.AuthenticateAsServerAsync(Certificate);
                    stream = secureStream;
                }

                // Create client record
                var client = Clients[endPoint] = new Client() {
                    RemoteEndPoint = endPoint,
                    Socket = socket,
                    Stream = stream,
                    LengthBuffer = new Byte[2],
                    LengthCount = 0
                };

                // Copy NoDelay
                socket.NoDelay = Underlying.NoDelay;

                // Raise connected event
                OnClientConnected?.Invoke(endPoint);

                // Start receiving
                ReceiveLength(client);

                // Start accepting next request
                AcceptStart();
            } catch (ObjectDisposedException) { };
        }

        private async Task ReceiveLength(Client client) {
            try {
                // Start the read
                var bytesTransfered = await client.Stream.ReadAsync(client.LengthBuffer, client.LengthCount, client.LengthBuffer.Length - client.LengthCount);

                // Detect closed connection and handle
                if (bytesTransfered <= 0) {
                    Closed(client.RemoteEndPoint);
                    return;
                }

                // Update byte received count with what was just received
                client.LengthCount += bytesTransfered;

                if (client.LengthCount < 2) {
                    // Not all length received - get more
                    ReceiveLength(client);
                } else {
                    // Compute length
                    var length = BitConverter.ToUInt16(client.LengthBuffer, 0);

                    // Abort if keep-alive message
                    if (length == 0) {
                        OnPokeReceived?.Invoke(client.RemoteEndPoint);
                        ReceiveLength(client);
                    }

                    // Allocate payload buffer
                    client.PayloadBuffer = new byte[length];

                    // Reset state
                    client.LengthCount = 0;

                    // Receive payload now
                    ReceivePayload(client);
                }
            } catch (ObjectDisposedException) { };
        }

        private async Task ReceivePayload(Client client) {
            try {
                // Start the read
                var bytesTransfered = await client.Stream.ReadAsync(client.PayloadBuffer, client.PayloadCount, client.PayloadBuffer.Length - client.PayloadCount);

                // Detect closed connection and handle
                if (bytesTransfered <= 0) {
                    Closed(client.RemoteEndPoint);
                    return;
                }

                // Update received count
                client.PayloadCount += bytesTransfered;

                if (client.PayloadCount < client.PayloadBuffer.Length) {
                    // Not all payload received - get more
                    ReceivePayload(client);
                } else {
                    // Instantiate message
                    var message = new TMessage();
                    message.Import(new ArraySegment<byte>(client.PayloadBuffer, 0, client.PayloadBuffer.Length));

                    // Reset state
                    client.PayloadCount = 0;

                    // Raise received event
                    OnMessageReceived?.Invoke(client.RemoteEndPoint, message);

                    // Restart receive process with next lenght header
                    ReceiveLength(client);
                }
            } catch (ObjectDisposedException) { };
        }

        private struct Client {
            public EndPoint RemoteEndPoint;
            public Socket Socket;
            public Stream Stream;

            public Byte[] LengthBuffer;
            public Int32 LengthCount;

            public Byte[] PayloadBuffer;
            public Int32 PayloadCount;

        }
    }


}
