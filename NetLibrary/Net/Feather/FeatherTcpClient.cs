using System;
using System.Collections.Generic;
using System.Text;

namespace InvertedTomato.Net.Feather
{
    class FeatherTcpClient
    {

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
        }
    }
}
