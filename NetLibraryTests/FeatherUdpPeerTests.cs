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
        private readonly Byte[] TestPayload = new byte[] { 1, 2, 3 };
        private readonly BinaryMessage TestMessage = new BinaryMessage(new byte[] { 1, 2, 3 });

        [Fact]
        public async Task SendMessageAsync() {
            var socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Loopback, 12345));

            using (var peer = new FeatherUdpPeer<BinaryMessage>()) {
                await peer.SendTo(new IPEndPoint(IPAddress.Loopback, 12345), TestMessage);
            }

            var buf = new byte[100];
            var len = socket.Receive(buf);

            Assert.Equal(TestPayload.Length, len);
            Assert.Equal(TestPayload, buf.Take(3));
        }

        [Fact]
        public void SendMessageSync() {
            var socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Loopback, 12346));

            using (var peer = new FeatherUdpPeer<BinaryMessage>()) {
                peer.SendToSync(new IPEndPoint(IPAddress.Loopback, 12346), TestMessage);
            }

            var buf = new byte[100];
            var len = socket.Receive(buf);

            Assert.Equal(TestPayload.Length, len);
            Assert.Equal(TestPayload, buf.Take(3));
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
                peer.SendToSync(new IPEndPoint(IPAddress.Loopback, 12346), TestMessage);
            });

            await Assert.ThrowsAsync<ObjectDisposedException>(async () => {
                await peer.SendTo(new IPEndPoint(IPAddress.Loopback, 12346), TestMessage);
            });
        }
    }
}
