using InvertedTomato.IO.Messages;
using InvertedTomato.Net.Feather;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Xunit;

namespace NetLibraryTests {
    public class FeatherTcpServerTests {
        private readonly Byte[] TestPayload1 = new byte[] { 1, 2, 3 };
        private readonly Byte[] TestWire1 = new byte[] { 3, 0, 1, 2, 3 };
        private readonly BinaryMessage TestMessage1 = new BinaryMessage(new byte[] { 1, 2, 3 });

        private readonly Byte[] TestPayload2 = new byte[] { 4, 5, 6 };
        private readonly Byte[] TestWire2 = new byte[] { 3, 0, 4, 5, 6 };
        private readonly BinaryMessage TestMessage2 = new BinaryMessage(new byte[] { 4, 5, 6 });

        private readonly Byte[] BlankWire = new byte[] { 0, 0 };

        [Fact]
        public void Send() {
            EndPoint remote = null;

            using (var server = new FeatherTcpServer<BinaryMessage>()) {
                server.Listen(12350);
                server.OnClientConnected += (endPoint) => {
                    remote = endPoint;
                };

                using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)) {
                    socket.NoDelay = true;
                    socket.Connect(new IPEndPoint(IPAddress.Loopback, 12350));

                    Thread.Sleep(10);
                    Assert.NotNull(remote);
                    server.SendTo(remote, TestMessage1);
                    server.SendTo(remote, TestMessage2);
                    Thread.Sleep(10);

                    var buffer = new byte[TestWire1.Length];
                    var pos = 0;
                    while (pos < buffer.Length) {
                        var len = socket.Receive(buffer, pos, TestWire1.Length - pos, SocketFlags.None);
                        pos += len;
                    }
                    Assert.Equal(TestWire1, buffer);

                    buffer = new byte[TestWire2.Length];
                    pos = 0;
                    while (pos < buffer.Length) {
                        var len = socket.Receive(buffer, pos, TestWire2.Length - pos, SocketFlags.None);
                        pos += len;
                    }
                    Assert.Equal(TestWire2, buffer);
                }
            }
        }

        [Fact]
        public void Disconnect() {
            var block = new AutoResetEvent(false);
            EndPoint remote = null;

            using (var server = new FeatherTcpServer<BinaryMessage>()) {
                server.OnClientConnected += (endPoint) => {
                    remote = endPoint;
                    block.Set();
                };
                server.Listen(12349);

                using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)) {
                    socket.NoDelay = true;
                    socket.Connect(new IPEndPoint(IPAddress.Loopback, 12349));
                    block.WaitOne(1000);

                    Assert.NotNull(remote);
                    server.Disconnect(remote);

                    Assert.Throws<SocketException>(() => {
                        for (var i = 0; i < 10; i++) {
                            socket.Send(new byte[] { 0, 0 });
                        }
                    });

                    Assert.False(socket.Connected);
                }
            }
        }

        [Fact]
        public void ClientConnectedDisconnected() {
            var stage = 0;
            var block = new AutoResetEvent(false);

            using (var server = new FeatherTcpServer<BinaryMessage>()) {
                server.OnClientConnected += (endPoint) => {
                    Assert.Equal(0, stage++);
                    Assert.NotNull(endPoint);
                };
                server.OnClientDisconnected += (endPoint, reason) => {
                    Assert.Equal(1, stage++);
                    Assert.NotNull(endPoint);
                    block.Set();
                };
                server.Listen(12349);

                using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)) {
                    socket.Connect(new IPEndPoint(IPAddress.Loopback, 12349));
                    Thread.Sleep(10);
                }

                block.WaitOne(1000);
                Assert.Equal(2, stage);
            }
        }

        [Fact]
        public void Receive() {
            var stage = 0;
            var block = new AutoResetEvent(false);

            using (var server = new FeatherTcpServer<BinaryMessage>()) {
                server.OnMessageReceived += (endpoint, message) => {
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
                server.Listen(12348);

                using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)) {
                    socket.NoDelay = true;
                    socket.Connect(new IPEndPoint(IPAddress.Loopback, 12348));
                    Assert.True(socket.Connected);
                    socket.Send(TestWire1);
                    Thread.Sleep(10);
                    socket.Send(TestWire2);

                    block.WaitOne(1000);
                }
            }

            Assert.Equal(2, stage);
        }

        [Fact]
        public void Receive_KeepAlive() {
            var state = 0;
            var block = new AutoResetEvent(false);

            using (var server = new FeatherTcpServer<BinaryMessage>()) {
                server.OnMessageReceived += (endpoint, message) => {
                    throw new Exception("Shouldn't receive blank message");
                };
                server.OnKeepAliveReceived += (endpoint) => {
                    Assert.NotNull(endpoint);
                    state++;
                    block.Set();
                };
                server.Listen(12351);

                using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)) {
                    socket.NoDelay = true;
                    socket.Connect(new IPEndPoint(IPAddress.Loopback, 12351));
                    Assert.True(socket.Connected);
                    socket.Send(BlankWire); // Blank message

                    block.WaitOne(1000);
                }
            }

            Assert.Equal(1, state);
        }

        [Fact]
        public void Dispose() {
            var server = new FeatherTcpServer<BinaryMessage>();
            Assert.False(server.IsDisposed);
            server.Listen(12345);
            Assert.False(server.IsDisposed);
            server.Dispose();
            Assert.True(server.IsDisposed);

            Assert.Throws<KeyNotFoundException>(() => {
                server.SendTo(new IPEndPoint(IPAddress.Loopback, 12346), TestMessage1);
            });

            // TODO SendToAsync
        }
    }
}
