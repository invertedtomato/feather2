using InvertedTomato.IO.Feather;
using System;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace InvertedTomato.Net.Feather {
    public sealed class Options {
        /// <summary>
        /// Use SSL to secure the connection.
        /// </summary>
        public Boolean IsSecure { get; set; } = false;

        /// <summary>
        /// Must be set if being used as a secure server, this certificate is used to prove identity to clients.
        /// </summary>
        public X509Certificate ServerCertificate { get; set; } = null;

        /// <summary>
        /// Must be set if being used as a secure client, this CN is used to verify the identity of the server.
        /// </summary>
        public String ServerCommonName { get; set; } = null;

        /// <summary>
        /// A keep-alive message will be sent after this amount of time if no other message has been sent. 
        /// </summary>
        public TimeSpan KeepAliveSendInterval { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// If no messages are received in this period it is assumed that there is a communication issue and the connection will be forcefully closed.
        /// </summary>
        public TimeSpan KeepAliveRequiredReciveInterval { get; set; } = TimeSpan.FromSeconds(15);

        /// <summary>
        /// Use the application-level keep-alive option instead of the standard TCP keep-alive. This works around buggy TCP implementations on some remote devices.
        /// </summary>
        public Boolean UseApplicationLayerKeepAlive { get; set; } = false;

        /// <summary>
        /// Size of receive buffer before blocking occurs.
        /// </summary>
        public Int32 ReceiveBufferSize { get; set; } = 8 * 1024; // bytes

        /// <summary>
		/// Size of send buffer before blocking occurs.
		/// </summary>
		public Int32 SendBufferSize { get; set; } = 8 * 1024; // bytes

        /// <summary>
        /// How lingering is handled.
        /// </summary>
        public LingerOption Linger { get; set; } = new LingerOption(true, 250);

        /// <summary>
        /// Maximum number of pending connections for a listener.
        /// </summary>
        public Int32 MaxListenBacklog { get; set; } = 16;

        /// <summary>
        /// Disable the Nagle algorithm to send data immediately, rather than delaying in the hopes of packing more data into packets.
        /// </summary>
        public Boolean NoDelay { get; set; } = false;
    }
}
