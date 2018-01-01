using InvertedTomato.IO.Feather;
using InvertedTomato.Net;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;

namespace InvertedTomato.Net.Feather {
    public class FeatherClient<TMessage> : FeatherStream<TMessage> where TMessage : IMessage, new() {
        private readonly Action<DisconnectionType> OnDisconnection;
        private readonly Action<TMessage> OnMessage;
        private readonly Options Options;
        private readonly Socket UnderlyingSocket;
        private readonly Thread ReceiveThread;

        public FeatherClient(String server) : this(ParseIPEndPoint(server)) { }

        public FeatherClient(EndPoint server) : this(server, new Options(), null, null) { }

        /// <summary>
        /// Connect to a Feather server.
        /// </summary>
        /// <returns>Server connection</returns>
        public FeatherClient(EndPoint server, Options options, Action<TMessage> onMessage, Action<DisconnectionType> onDisconnection) : base(true) {
            if(null == server) {
                throw new ArgumentNullException(nameof(server));
            }
            if(null == options) {
                throw new ArgumentNullException(nameof(options));
            }
            if(null == onMessage) {
                throw new ArgumentNullException(nameof(onMessage));
            }
            if(null == onDisconnection) {
                throw new ArgumentNullException(nameof(onDisconnection));
            }

            // Store
            Options = options;
            OnMessage = onMessage;
            OnDisconnection = onDisconnection;

            // Open socket
            UnderlyingSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            UnderlyingSocket.SetKeepAlive(Options.KeepAliveSendInterval, Options.KeepAliveRequiredReciveInterval);
            UnderlyingSocket.ReceiveBufferSize = Options.ReceiveBufferSize;
            UnderlyingSocket.SendBufferSize = Options.SendBufferSize;
            UnderlyingSocket.LingerState = Options.Linger;
            UnderlyingSocket.NoDelay = Options.NoDelay;
            UnderlyingSocket.Connect(server);

            // Get stream
            Underlying = new NetworkStream(UnderlyingSocket, true);
            if(Options.IsSecure) {
                var secureStream = new SslStream(Underlying, false);
                secureStream.AuthenticateAsClientAsync(Options.ServerCommonName).Wait();
                Underlying = secureStream;
            }

            // Seed receiving
            ReceiveThread = new Thread(ReceiveThread_OnSpin);
            ReceiveThread.Start();
        }

        public FeatherClient(Boolean isServer, Socket socket, Options options, Action<TMessage> onMessage, Action<DisconnectionType> onDisconnection) : base(true) {
            if(null == socket) {
                throw new ArgumentNullException(nameof(socket));
            }
            if(null == options) {
                throw new ArgumentNullException(nameof(options));
            }
            if(null == onMessage) {
                throw new ArgumentNullException(nameof(onMessage));
            }
            if(null == onDisconnection) {
                throw new ArgumentNullException(nameof(onDisconnection));
            }

            // Store
            UnderlyingSocket = socket;
            Options = options;
            OnMessage = onMessage;
            OnDisconnection = onDisconnection;

            // Get stream
            Underlying = (Stream)new NetworkStream(UnderlyingSocket, true);
            if(Options.IsSecure) {
                var secureStream = new SslStream(Underlying, false);
                if(isServer) {
                    secureStream.AuthenticateAsServerAsync(Options.ServerCertificate).Wait();
                } else {
                    secureStream.AuthenticateAsClientAsync(Options.ServerCommonName).Wait();
                }
                Underlying = secureStream;
            }
            
            // Seed receiving
            ReceiveThread = new Thread(ReceiveThread_OnSpin);
            ReceiveThread.Start();
        }

        
        private void ReceiveThread_OnSpin(Object obj) {
            try {
                while(!IsDisposed) {
                    var message = Read(); // TODO: Timeout?
                    OnMessage(message);
                }
            } catch(ObjectDisposedException) {
            } catch(IOException) {
                DisconnectInner(DisconnectionType.ConnectionInterupted);
            }
        }

        public override TMessage Read() {
            try {
                return base.Read();
            } catch(IOException) {
                DisconnectInner(DisconnectionType.ConnectionInterupted);
                throw;
            }
        }

        public override void Write(TMessage message) {
            try {
                 base.Write(message);
            } catch(IOException) {
                DisconnectInner(DisconnectionType.ConnectionInterupted);
                throw;
            }
        }

        /// <summary>
        /// Disconnect from the remote endpoint and dispose.
        /// </summary>
        public void Disconnect() {
            DisconnectInner(DisconnectionType.LocalDisconnection);
        }
        

        /// <summary>
        /// Handle internal disconnect requests.
        /// </summary>
        /// <param name="reason"></param>
        private void DisconnectInner(DisconnectionType reason) {
            if(IsDisposed) {
                return;
            }
            Dispose();

            OnDisconnection?.Invoke(reason);
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        private new void Dispose(Boolean disposing) {
            if(IsDisposed) {
                return;
            }

            if(disposing) {
                base.Dispose();
                UnderlyingSocket?.Shutdown(SocketShutdown.Both);
                UnderlyingSocket?.Dispose();
                ReceiveThread.Join();
            }
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public new void Dispose() {
            Dispose(true);
        }

        private static IPEndPoint ParseIPEndPoint(String value) {
            if(null == value) {
                throw new ArgumentNullException(nameof(value));
            }

            var ep = value.Split(':');
            if(ep.Length < 2) {
                throw new FormatException("Invalid endpoint format");
            }

            IPAddress ip;
            if(ep.Length > 2) {
                if(!IPAddress.TryParse(string.Join(":", ep, 0, ep.Length - 1), out ip)) {
                    throw new FormatException("Invalid ip-adress");
                }
            } else {
                if(!IPAddress.TryParse(ep[0], out ip)) {
                    throw new FormatException("Invalid ip-adress");
                }
            }

            Int32 port;
            if(!int.TryParse(ep[ep.Length - 1], NumberStyles.None, NumberFormatInfo.CurrentInfo, out port)) {
                throw new FormatException("Invalid port");
            }

            return new IPEndPoint(ip, port);
        }
    }
}
