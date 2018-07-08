using InvertedTomato.IO.Messages;
using InvertedTomato.Net.Feather;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace NetLibraryTests {
    public class FeatherUdpPeerTests {
        private readonly Byte[] TestPayload1 = new byte[] { 1, 2, 3 };
        private readonly BinaryMessage TestMessage1 = new BinaryMessage(new byte[] { 1, 2, 3 });

        private readonly Byte[] TestPayload2 = new byte[] { 4,5,6 };
        private readonly BinaryMessage TestMessage2 = new BinaryMessage(new byte[] { 4,5,6 });

        [Fact]
        public async Task SendMessageAsync() {
            var socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Loopback, 12345));

            using (var peer = new FeatherUdpPeer<BinaryMessage>()) {
                await peer.SendTo(new IPEndPoint(IPAddress.Loopback, 12345), TestMessage1);
                await peer.SendTo(new IPEndPoint(IPAddress.Loopback, 12345), TestMessage2);
            }

            byte[] buf = new byte[100];
            Int32 len;

            len = socket.Receive(buf);
            Assert.Equal(TestPayload1.Length, len);
            Assert.Equal(TestPayload1, buf.Take(3));

            len = socket.Receive(buf);
            Assert.Equal(TestPayload2.Length, len);
            Assert.Equal(TestPayload2, buf.Take(3));
        }

        [Fact]
        public void SendMessageSync() {
            var socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Loopback, 12346));

            using (var peer = new FeatherUdpPeer<BinaryMessage>()) {
                peer.SendToSync(new IPEndPoint(IPAddress.Loopback, 12346), TestMessage1);
                peer.SendToSync(new IPEndPoint(IPAddress.Loopback, 12346), TestMessage2);
            }

            byte[] buf = new byte[100];
            Int32 len;

            len = socket.Receive(buf);
            Assert.Equal(TestPayload1.Length, len);
            Assert.Equal(TestPayload1, buf.Take(3));

            len = socket.Receive(buf);
            Assert.Equal(TestPayload2.Length, len);
            Assert.Equal(TestPayload2, buf.Take(3));
        }

        [Fact]
        public void ReceiveMessage() {
            throw new NotImplementedException();
        }

        [Fact]
        public async void Dispose() {
            var peer = new FeatherUdpPeer<BinaryMessage>();
            Assert.False(peer.IsDisposed);
            peer.Bind(12347);
            Assert.False(peer.IsDisposed);
            peer.Dispose();
            Assert.True(peer.IsDisposed);

            Assert.Throws<ObjectDisposedException>(() => {
                peer.SendToSync(new IPEndPoint(IPAddress.Loopback, 12346), TestMessage1);
            });

            await Assert.ThrowsAsync<ObjectDisposedException>(async () => {
                await peer.SendTo(new IPEndPoint(IPAddress.Loopback, 12346), TestMessage1);
            });
        }
    }
}
