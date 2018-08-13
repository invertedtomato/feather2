using InvertedTomato.IO.Messages;
using InvertedTomato.Net.Feather;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        public async void Send () {
            var block = new AutoResetEvent(false);
            EndPoint remote = null;

            using (var server = new FeatherTcpServer<BinaryMessage>()) {
                server.OnClientConnected += (endPoint) => {
                    remote = endPoint;
                    block.Set();
                };
                server.Listen(13000);

                using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)) {
                    socket.NoDelay = true;
                    socket.Connect(new IPEndPoint(IPAddress.Loopback, 13000));

                    block.WaitOne(1000);
                    Assert.NotNull(remote);
                    await server.SendToAsync(remote, TestMessage1);
                    await server.SendToAsync(remote, TestMessage2);

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
        public async void SendAsync () {
            var block = new AutoResetEvent(false);
            EndPoint remote = null;

            using (var server = new FeatherTcpServer<BinaryMessage>()) {
                server.Listen(13001);
                server.OnClientConnected += (endPoint) => {
                    remote = endPoint;
                    block.Set();
                };

                using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)) {
                    socket.NoDelay = true;
                    socket.Connect(new IPEndPoint(IPAddress.Loopback, 13001));

                    block.WaitOne(1000);
                    Assert.NotNull(remote);
                    await server.SendToAsync(remote, TestMessage1);
                    await server.SendToAsync(remote, TestMessage2);

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

        /*
        [Fact]
        public void Poke() {
            EndPoint remote = null;

            using (var server = new FeatherTcpServer<BinaryMessage>()) {
                server.Listen(13002);
                server.OnClientConnected += (endPoint) => {
                    remote = endPoint;
                };

                using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)) {
                    socket.NoDelay = true;
                    socket.Connect(new IPEndPoint(IPAddress.Loopback, 13002));

                    Thread.Sleep(10);
                    Assert.NotNull(remote);
                    server.Poke(remote);
                    Thread.Sleep(10);

                    var buffer = new byte[BlankWire.Length];
                    var pos = 0;
                    while (pos < buffer.Length) {
                        var len = socket.Receive(buffer, pos, BlankWire.Length - pos, SocketFlags.None);
                        pos += len;
                    }
                    Assert.Equal(BlankWire, buffer);
                }
            }
        }
        */

        [Fact]
        public void Disconnect () {
            var block = new AutoResetEvent(false);
            EndPoint remote = null;

            using (var server = new FeatherTcpServer<BinaryMessage>()) {
                server.OnClientConnected += (endPoint) => {
                    remote = endPoint;
                    block.Set();
                };
                server.Listen(13003);

                using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)) {
                    socket.NoDelay = true;
                    socket.Connect(new IPEndPoint(IPAddress.Loopback, 13003));
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
        public void ClientConnectedDisconnected () {
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
                server.Listen(13004);

                using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)) {
                    socket.Connect(new IPEndPoint(IPAddress.Loopback, 13004));
                    Thread.Sleep(10);
                }

                block.WaitOne(1000);
                Assert.Equal(2, stage);
            }
        }

        [Fact]
        public void Receive () {
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
                server.Listen(13005);

                using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)) {
                    socket.NoDelay = true;
                    socket.Connect(new IPEndPoint(IPAddress.Loopback, 13005));
                    Assert.True(socket.Connected);
                    socket.Send(TestWire1);
                    Thread.Sleep(10);
                    socket.Send(TestWire2);

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

            using (var server = new FeatherTcpServer<BinaryMessage>()) {
                server.OnMessageReceived += (endpoint, message) => {
                    throw new Exception("Shouldn't receive blank message");
                };
                server.OnPokeReceived += (endpoint) => {
                    Assert.NotNull(endpoint);
                    state++;
                    block.Set();
                };
                server.Listen(13006);

                using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)) {
                    socket.NoDelay = true;
                    socket.Connect(new IPEndPoint(IPAddress.Loopback, 13006));
                    Assert.True(socket.Connected);
                    socket.Send(BlankWire); // Blank message

                    block.WaitOne(1000);
                }
            }

            Assert.Equal(1, state);
        }
        */

        [Fact]
        public void Dispose () {
            var server = new FeatherTcpServer<BinaryMessage>();
            Assert.False(server.IsDisposed);
            server.Listen(13007);
            Assert.False(server.IsDisposed);
            server.Dispose();
            Assert.True(server.IsDisposed);
        }

        [Fact]
        public async Task Secure () {
            var state = 0;
            var block = new AutoResetEvent(false);

            var raw = File.ReadAllBytes("localhost.pfx");
            var certificate = new X509Certificate2(raw, "test");

            using (var server = new FeatherTcpServer<BinaryMessage>()) {
                server.OnMessageReceived += (endPoint, message) => {
                    if (state == 0) {
                        Assert.Equal(TestPayload1, message.Export().ToArray());
                        state++;
                    } else if (state == 1) {
                        Assert.Equal(TestPayload2, message.Export().ToArray());
                        state++;
                        block.Set();
                    } else {
                        throw new Exception();
                    }
                };
                server.ListenSecure(13008, certificate);

                using (var client = new FeatherTcpClient<BinaryMessage>()) {
                    await client.ConnectSecureAsync("localhost", 13008);
                    await client.SendAsync(TestMessage1);
                    await client.SendAsync(TestMessage2);

                    block.WaitOne(1000);
                }
            }

            Assert.Equal(2, state);
        }
    }
}
