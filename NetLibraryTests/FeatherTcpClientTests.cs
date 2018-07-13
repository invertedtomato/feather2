using InvertedTomato.IO.Messages;
using InvertedTomato.Net.Feather;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Xunit;

namespace NetLibraryTests {
    public class FeatherTcpClientTests {
        private readonly Byte[] TestPayload1 = new byte[] { 1, 2, 3 };
        private readonly Byte[] TestWire1 = new byte[] { 3, 0, 1, 2, 3 };
        private readonly BinaryMessage TestMessage1 = new BinaryMessage(new byte[] { 1, 2, 3 });

        private readonly Byte[] TestPayload2 = new byte[] { 4, 5, 6 };
        private readonly Byte[] TestWire2 = new byte[] { 3, 0, 4, 5, 6 };
        private readonly BinaryMessage TestMessage2 = new BinaryMessage(new byte[] { 4, 5, 6 });

        // TODO: Send, receive!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        [Fact]
        public void Dispose() {
            var server = new FeatherTcpClient<BinaryMessage>();
            Assert.False(server.IsDisposed);
            server.Dispose();
            Assert.True(server.IsDisposed);

            Assert.Throws<ObjectDisposedException>(() => {
                server.Send(TestMessage1);
            });

            // TODO SendToAsync
        }
    }
}
