using InvertedTomato.IO.Messages;
using InvertedTomato.Net.Feather;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace NetLibraryTests {
    public class FeatherTcpServerTests {
        private readonly Byte[] TestPayload1 = new byte[] { 1, 2, 3 };
        private readonly BinaryMessage TestMessage1 = new BinaryMessage(new byte[] { 1, 2, 3 });

        private readonly Byte[] TestPayload2 = new byte[] { 4, 5, 6 };
        private readonly BinaryMessage TestMessage2 = new BinaryMessage(new byte[] { 4, 5, 6 });

        [Fact]
        public void Receive() {
            /*
            using(var server = new FeatherTcpServer<BinaryMessage>()) {

            }

            var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Loopback, 12346));
            socket.Listen(1);*/
        }

        [Fact]
        public void Dispose() {
            var server = new FeatherTcpServer<BinaryMessage>();
            Assert.False(server.IsDisposed);
            server.Listen(12345);
            Assert.False(server.IsDisposed);
            server.Dispose();
            Assert.True(server.IsDisposed);

            Assert.Throws<ObjectDisposedException>(() => {
                server.SendTo(new IPEndPoint(IPAddress.Loopback, 12346), TestMessage1);
            });

            // TODO SendToAsync
        }
    }
}
