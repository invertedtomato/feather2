using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ThreePlay.IO.Zero;
using ThreePlay.IO.Zero.Messages;

namespace ThreePlay.Net.Zero {
    public sealed class ZeroUDP {
        /// <summary>
        /// Send a message to a Zero server.
        /// </summary>
        public void Send(IPAddress serverAddress, int port, Message message) { Send(new IPEndPoint(serverAddress, port), message); }

        /// <summary>
        /// Send a message to a Zero server.
        /// </summary>
        public void Send(IPAddress serverAddress, int port, Message message, ZeroUDPOptions options) { Send(new IPEndPoint(serverAddress, port), message, options); }

        /// <summary>
        /// Send a message to a Zero server.
        /// </summary>
        public void Send(string serverName, int port, Message message) { Send(new DnsEndPoint(serverName, port), message); }

        /// <summary>
        /// Send a message to a Zero server.
        /// </summary>
        public void Send(string serverName, int port, Message message, ZeroUDPOptions options) { Send(new DnsEndPoint(serverName, port), message, options); }

        /// <summary>
        /// Send a message to a Zero server.
        /// </summary>
        public void Send(EndPoint endPoint, Message message) { Send(endPoint, message, new ZeroUDPOptions()); }

        /// <summary>
        /// Send a message to a Zero server.
        /// </summary>
        public void Send(EndPoint endPoint, Message message, ZeroUDPOptions options) {
            if (null == endPoint) {
                throw new ArgumentNullException("endPoint");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }

            // Create message
            var buffer = message.ToByteArray(); // DO NOT USE Core.ZeroPayloadsToBuffer - not required for UDP

            // Open socket
            using (var clientSocket = new Socket(SocketType.Stream, ProtocolType.Udp)) {
                clientSocket.SendTo(buffer, endPoint);
            }
        }




        public static ZeroUDP Start() { return Start(new ZeroUDPOptions()); }

        public static ZeroUDP Start(ZeroUDPOptions options) {

            throw new NotImplementedException();
        }

        /// <summary>
        /// Start a Zero server by listening for messages.
        /// </summary>
        /// <returns>Zero instance</returns>
        public static ZeroUDP StartAndBind(int bindPort) { return StartAndBind(new IPEndPoint(IPAddress.Any, bindPort)); }

        /// <summary>
        /// Start a Zero server by listening for messages.
        /// </summary>
        /// <returns>Zero instance</returns>
        public static ZeroUDP StartAndBind(int bindPort, ZeroUDPOptions options) { return StartAndBind(new IPEndPoint(IPAddress.Any, bindPort), options); }

        /// <summary>
        /// Start a Zero server by listening for messages.
        /// </summary>
        /// <returns>Zero instance</returns>
        public static ZeroUDP StartAndBind(EndPoint localEndPoint) { return StartAndBind(localEndPoint, new ZeroUDPOptions()); }

        /// <summary>
        /// Start a Zero server by listening for messages.
        /// </summary>
        /// <returns>Zero instance</returns>
        public static ZeroUDP StartAndBind(EndPoint localEndPoint, ZeroUDPOptions options) {
            if (null == localEndPoint) {
                throw new ArgumentNullException("endpoint");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }


            throw new NotImplementedException();

            return new ZeroUDP(localEndPoint, options);
        }
    }
}
