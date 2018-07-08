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
        [Fact]
        public async Task SendMessageAsync() {
            var socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Loopback, 12345));

            using (var peer = new FeatherUdpPeer<BinaryMessage>()) {
                var msg = new BinaryMessage(new byte[] { 1, 2, 3 });

                await peer.SendTo(new IPEndPoint(IPAddress.Loopback, 12345), msg);
                Thread.Sleep(100);
            }

            var buf = new byte[100];
            var len = socket.Receive(buf);

            Assert.Equal(3, len);
            Assert.Equal(new byte[] { 1, 2, 3 }, buf.Take(3));
        }

        [Fact]
        public void SendMessageSync() {
            var socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Loopback, 12346));

            using (var peer = new FeatherUdpPeer<BinaryMessage>()) {
                var msg = new BinaryMessage(new byte[] { 1, 2, 3 });

                peer.SendToSync(new IPEndPoint(IPAddress.Loopback, 12346), msg);
                Thread.Sleep(100);
            }

            var buf = new byte[100];
            var len = socket.Receive(buf);

            Assert.Equal(3, len);
            Assert.Equal(new byte[] { 1, 2, 3 }, buf.Take(3));
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
                peer.SendToSync(new IPEndPoint(IPAddress.Loopback, 12346), new BinaryMessage(new byte[] { 1, 2, 3 }));
            });

            await Assert.ThrowsAsync<ObjectDisposedException>(async () => {
                await peer.SendTo(new IPEndPoint(IPAddress.Loopback, 12346), new BinaryMessage(new byte[] { 1, 2, 3 }));
            });
        }
    }
}
