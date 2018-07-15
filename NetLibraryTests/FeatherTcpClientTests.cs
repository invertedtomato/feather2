using InvertedTomato.IO.Messages;
using InvertedTomato.Net.Feather;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NetLibraryTests {
    public class FeatherTcpClientTests {
        private readonly Byte[] TestPayload1 = new byte[] { 1, 2, 3 };
        private readonly Byte[] TestWire1 = new byte[] { 3, 0, 1, 2, 3 };
        private readonly BinaryMessage TestMessage1 = new BinaryMessage(new byte[] { 1, 2, 3 });

        private readonly Byte[] TestPayload2 = new byte[] { 4, 5, 6 };
        private readonly Byte[] TestWire2 = new byte[] { 3, 0, 4, 5, 6 };
        private readonly BinaryMessage TestMessage2 = new BinaryMessage(new byte[] { 4, 5, 6 });

        private readonly Byte[] BlankWire = new byte[] { 0, 0 };

        [Fact]
        public void Send() {
            using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)) {
                socket.NoDelay = true;
                socket.Bind(new IPEndPoint(IPAddress.Loopback, 12350));
                socket.Listen(1);

                using (var client = new FeatherTcpClient<BinaryMessage>()) {
                    client.Connect("127.0.0.1", 12350);

                    var socketa = socket.Accept();
                    client.Send(TestMessage1);
                    client.Send(TestMessage2);
                    Thread.Sleep(10);

                    var buffer = new byte[TestWire1.Length];
                    var pos = 0;
                    while (pos < buffer.Length) {
                        var len = socketa.Receive(buffer, pos, TestWire1.Length - pos, SocketFlags.None);
                        pos += len;
                    }
                    Assert.Equal(TestWire1, buffer);

                    buffer = new byte[TestWire2.Length];
                    pos = 0;
                    while (pos < buffer.Length) {
                        var len = socketa.Receive(buffer, pos, TestWire2.Length - pos, SocketFlags.None);
                        pos += len;
                    }
                    Assert.Equal(TestWire2, buffer);
                }
            }
        }

        [Fact]
        public async void SendAsync() {
            using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)) {
                socket.NoDelay = true;
                socket.Bind(new IPEndPoint(IPAddress.Loopback, 12350));
                socket.Listen(1);

                using (var client = new FeatherTcpClient<BinaryMessage>()) {
                    client.Connect("127.0.0.1", 12350);

                    var socketa = socket.Accept();
                    await client.SendAsync(TestMessage1);
                    await client.SendAsync(TestMessage2);
                    Thread.Sleep(10);

                    var buffer = new byte[TestWire1.Length];
                    var pos = 0;
                    while (pos < buffer.Length) {
                        var len = socketa.Receive(buffer, pos, TestWire1.Length - pos, SocketFlags.None);
                        pos += len;
                    }
                    Assert.Equal(TestWire1, buffer);

                    buffer = new byte[TestWire2.Length];
                    pos = 0;
                    while (pos < buffer.Length) {
                        var len = socketa.Receive(buffer, pos, TestWire2.Length - pos, SocketFlags.None);
                        pos += len;
                    }
                    Assert.Equal(TestWire2, buffer);
                }
            }
        }

        /*
        [Fact]
        public void Poke() {
            using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)) {
                socket.NoDelay = true;
                socket.Bind(new IPEndPoint(IPAddress.Loopback, 12355));
                socket.Listen(1);

                using (var client = new FeatherTcpClient<BinaryMessage>()) {
                    client.Connect("127.0.0.1", 12355);

                    var socketa = socket.Accept();
                    Assert.True(socketa.Connected);
                    client.Poke();
                    Thread.Sleep(10);

                    var buffer = new byte[BlankWire.Length];
                    var pos = 0;
                    while (pos < buffer.Length) {
                        var len = socketa.Receive(buffer, pos, BlankWire.Length - pos, SocketFlags.None);
                        pos += len;
                    }
                    Assert.Equal(BlankWire, buffer);
                }
            }
        }
        */

        [Fact]
        public void ClientDisconnected() {
            var stage = 0;
            var block = new AutoResetEvent(false);

            using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)) {
                socket.NoDelay = true;
                socket.Bind(new IPEndPoint(IPAddress.Loopback, 12356));
                socket.Listen(1);

                using (var client = new FeatherTcpClient<BinaryMessage>()) {
                    client.OnDisconnected += (reason) => {
                        Assert.Equal(0, stage++);
                        block.Set();
                    };
                    client.Connect("127.0.0.1", 12356);
                    var socketa = socket.Accept();

                    Thread.Sleep(10);
                    socketa.Shutdown(SocketShutdown.Both);
                    socketa.Dispose();

                    block.WaitOne(1000);
                    Assert.Equal(1, stage);
                }
            }
        }

        [Fact]
        public void Receive() {
            var stage = 0;
            var block = new AutoResetEvent(false);

            using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)) {
                socket.NoDelay = true;
                socket.Bind(new IPEndPoint(IPAddress.Loopback, 12357));
                socket.Listen(1);

                using (var client = new FeatherTcpClient<BinaryMessage>()) {
                    client.OnMessageReceived += (message) => {
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
                    client.Connect("127.0.0.1", 12357);
                    var socketa = socket.Accept();
                    Assert.True(socketa.Connected);
                    socketa.Send(TestWire1);
                    Thread.Sleep(10);
                    socketa.Send(TestWire2);

                    block.WaitOne(1000);
                }
            }

            Assert.Equal(2, stage);
        }
        /*
        [Fact]
        public void ReceivePoke() {
            var state = 0;
            var block = new AutoResetEvent(false);

            using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)) {
                socket.NoDelay = true;
                socket.Bind(new IPEndPoint(IPAddress.Loopback, 12358));
                socket.Listen(1);

                using (var client = new FeatherTcpClient<BinaryMessage>()) {
                    client.OnMessageReceived += (message) => {
                        throw new Exception("Shouldn't receive blank message");
                    };
                    client.OnPokeReceived += () => {
                        state++;
                        block.Set();
                    };
                    client.Connect("127.0.0.1", 12358);
                    var socketa = socket.Accept();

                    Assert.True(socketa.Connected);
                    socketa.Send(BlankWire); // Blank message

                    block.WaitOne(1000);

                }

                Assert.Equal(1, state);
            }
        }
        */
        [Fact]
        public async Task Dispose() {
            using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)) {
                socket.NoDelay = true;
                socket.Bind(new IPEndPoint(IPAddress.Loopback, 12359));
                socket.Listen(1);

                var client = new FeatherTcpClient<BinaryMessage>();
                client.Connect("127.0.0.1", 12359);
                Assert.False(client.IsDisposed);
                client.Dispose();
                Assert.True(client.IsDisposed);

                Assert.Throws<ObjectDisposedException>(() => {
                    client.Send(TestMessage1);
                });

                await Assert.ThrowsAsync<ObjectDisposedException>(async () => {
                    await client.SendAsync(TestMessage1);
                });
            }
        }


        [Fact]
        public void Send_NotConnected() {
            var client = new FeatherTcpClient<BinaryMessage>();
            Assert.Throws<InvalidOperationException>(() => {
                client.Send(TestMessage1);
            });
        }
    }
}