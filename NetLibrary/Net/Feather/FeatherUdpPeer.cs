using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using InvertedTomato.IO.Messages;

namespace InvertedTomato.Net.Feather {
    public class FeatherUdpPeer<TMessage> : IDisposable where TMessage : IImportableMessage, IExportableMessage, new() {
        private readonly Socket Underlying = new Socket(SocketType.Dgram, ProtocolType.Udp);
        private readonly Object Sync = new Object();

        /// <summary>
        /// Fired when a message is recieved. The message is provided, along with the remote endpoint that sent the message.
        /// </summary>
        public event Action<EndPoint, TMessage> OnMessageReceived;

        /// <summary>
        /// The maximum message size that will be accepted. If the message is less than this size it will be truncaded. Note that over the internet using IPv4 508 is the maximum safe size, and 1,212 over IPv6.
        /// </summary>
        public Int32 ReceiveMaxMessageSize { get; set; } = 1500;

        /// <summary>
        /// Is it disposed?
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Bind to a local port on all IP addresses.
        /// </summary>
        /// <param name="port"></param>
        public void Bind(Int32 port) {
            Bind(new IPEndPoint(IPAddress.Any, port));
        }

        /// <summary>
        /// Bind to a local port on a specified IP address.
        /// </summary>
        /// <param name="localEndPoint"></param>
        public void Bind(EndPoint localEndPoint) {
            lock (Sync) {
                // Bind underlying socket
                Underlying.Bind(localEndPoint);

                // Start receiving
                Receive();
            }
        }

        /// <summary>
        /// Send a message to a given remote endpoint asynchronously.
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendToAsync(EndPoint remoteEndPoint, TMessage message) {
            if (null == remoteEndPoint) {
                throw new ArgumentNullException(nameof(remoteEndPoint));
            }
            if (null == message) {
                throw new ArgumentNullException(nameof(message));
            }

            // Extract payload from message
            var payload = message.Export();

            // Send!
            await Underlying.SendToAsync(payload, SocketFlags.None, remoteEndPoint);
        }

        /// <summary>
        /// Send a message to a given remote endpoint synchronously.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="message"></param>
        public void SendTo(EndPoint target, TMessage message) {
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



        private void Receive() {
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

                Receive();
            };

            try {
                Underlying.ReceiveMessageFromAsync(args);
            } catch (ObjectDisposedException) { };
        }
    }
}