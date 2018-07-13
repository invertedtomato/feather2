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

        private readonly Byte[] TestPayload2 = new byte[] { 4, 5, 6 };
        private readonly BinaryMessage TestMessage2 = new BinaryMessage(new byte[] { 4, 5, 6 });

        [Fact]
        public void SendMessageSync() {
            var socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Loopback, 12346));

            using (var peer = new FeatherUdpPeer<BinaryMessage>()) {
                peer.SendTo(new IPEndPoint(IPAddress.Loopback, 12346), TestMessage1);
                peer.SendTo(new IPEndPoint(IPAddress.Loopback, 12346), TestMessage2);
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
        public async Task SendMessageAsync() {
            var socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Loopback, 12345));

            using (var peer = new FeatherUdpPeer<BinaryMessage>()) {
                await peer.SendToAsync(new IPEndPoint(IPAddress.Loopback, 12345), TestMessage1);
                await peer.SendToAsync(new IPEndPoint(IPAddress.Loopback, 12345), TestMessage2);
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
            var stage = 0;
            var block = new AutoResetEvent(false);

            using (var peer = new FeatherUdpPeer<BinaryMessage>()) {
                peer.OnMessageReceived += (endpoint, message) => {
                    if (stage == 0) {
                        Assert.Equal(TestPayload1, message.Export().ToArray());
                        stage = 1;
                    } else if (stage == 1) {
                        Assert.Equal(TestPayload2, message.Export().ToArray());
                        stage = 2;
                        block.Set();
                    } else {
                        Assert.False(true);
                    }
                };
                peer.Bind(12348);

                using (var socket = new Socket(SocketType.Dgram, ProtocolType.Udp)) {
                    socket.SendTo(TestPayload1, new IPEndPoint(IPAddress.Loopback, 12348));
                    Thread.Sleep(10);
                    socket.SendTo(TestPayload2, new IPEndPoint(IPAddress.Loopback, 12348));
                }
            }

            block.WaitOne(1000);

            Assert.Equal(2, stage);
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
                peer.SendTo(new IPEndPoint(IPAddress.Loopback, 12346), TestMessage1);
            });

            await Assert.ThrowsAsync<ObjectDisposedException>(async () => {
                await peer.SendToAsync(new IPEndPoint(IPAddress.Loopback, 12346), TestMessage1);
            });
        }
    }
}
