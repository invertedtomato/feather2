using InvertedTomato.IO.Messages;
using InvertedTomato.Net.Feather;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
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
        public void Send() {
            var block = new AutoResetEvent(false);
            EndPoint remote = null;

            using (var server = new FeatherTcpServer<BinaryMessage>()) {
                server.OnClientConnected += (endPoint) => {
                    remote = endPoint;
                    block.Set();
                };
                server.Listen(12350);

                using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)) {
                    socket.NoDelay = true;
                    socket.Connect(new IPEndPoint(IPAddress.Loopback, 12350));

                    block.WaitOne(1000);
                    Assert.NotNull(remote);
                    server.SendTo(remote, TestMessage1);
                    server.SendTo(remote, TestMessage2);

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
        public async void SendAsync() {
            var block = new AutoResetEvent(false);
            EndPoint remote = null;

            using (var server = new FeatherTcpServer<BinaryMessage>()) {
                server.Listen(12350);
                server.OnClientConnected += (endPoint) => {
                    remote = endPoint;
                    block.Set();
                };

                using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)) {
                    socket.NoDelay = true;
                    socket.Connect(new IPEndPoint(IPAddress.Loopback, 12350));

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
                server.Listen(12352);
                server.OnClientConnected += (endPoint) => {
                    remote = endPoint;
                };

                using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)) {
                    socket.NoDelay = true;
                    socket.Connect(new IPEndPoint(IPAddress.Loopback, 12352));

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
        */

        [Fact]
        public void Dispose() {
            var server = new FeatherTcpServer<BinaryMessage>();
            Assert.False(server.IsDisposed);
            server.Listen(12345);
            Assert.False(server.IsDisposed);
            server.Dispose();
            Assert.True(server.IsDisposed);
        }

        [Fact]
        public async Task Secure() {
            var state = 0;
            var block = new AutoResetEvent(false);

            var certificateBody = "-----BEGIN CERTIFICATE-----" +
            "MIIDnTCCAoSgAwIBAgIBADANBgkqhkiG9w0BAQ0FADBoMQswCQYDVQQGEwJhdTET" +
            "MBEGA1UECAwKUXVlZW5zbGFuZDEQMA4GA1UECgwHRmVhdGhlcjESMBAGA1UEAwwJ" +
            "bG9jYWxob3N0MREwDwYDVQQHDAhCcmlzYmFuZTELMAkGA1UECwwCSVQwHhcNMTgw" +
            "NzE1MDUwMzM3WhcNMjEwNDEwMDUwMzM3WjBoMQswCQYDVQQGEwJhdTETMBEGA1UE" +
            "CAwKUXVlZW5zbGFuZDEQMA4GA1UECgwHRmVhdGhlcjESMBAGA1UEAwwJbG9jYWxo" +
            "b3N0MREwDwYDVQQHDAhCcmlzYmFuZTELMAkGA1UECwwCSVQwggEjMA0GCSqGSIb3" +
            "DQEBAQUAA4IBEAAwggELAoIBAgC6Bj4C/UNEOD18o7M8VUGbLLYQaSSabBREKE9D" +
            "IAb9aZ4Tue9G3guQaX4l3frkm2MCVVqDst3U/suQQ1Q+kva+TOC5mM0Sqte9pF76" +
            "/jCHieeH+7bwnqe07atys4M+3UgjDBxV8d0sXeBM9XTR2lV0e6awUr2101yLqMlG" +
            "NGA+U8sRTPg9gnWyH2JYMkwy16nNR6AYrxXAi9XHBRYI++Dty+uLFYRF03yvC4+m" +
            "4z7Lc/tfS1jECbAwexjQXKdAa9E2YVqXPNtgqvOz5rRMMbF8MclY+COAoPRJm25J" +
            "AElT+vZZwrxIw9C34HbPKcXsZ7TjzrwTKMFSq0xi394lsnlVxQIDAQABo1AwTjAd" +
            "BgNVHQ4EFgQUe/3r7fIVV44QtqhLmrX2ZNei51gwHwYDVR0jBBgwFoAUe/3r7fIV" +
            "V44QtqhLmrX2ZNei51gwDAYDVR0TBAUwAwEB/zANBgkqhkiG9w0BAQ0FAAOCAQIA" +
            "MmZYCp2Jw/M5rXfmcKBbaxi73R9sExVOb4HMi0HjI9fKsaOqf+vWDklx4OlMLWWU" +
            "Kwx/5TLl7J8tiABxtvc9OtuQdCbx0p42fdfjf1hxe+D0gcsLXoAvR4y7BZITcvMN" +
            "6QOGsFCb0SKfkx/HagK9GwMYuVH3pxjwZJtvdWmq5EZuYuOtiEy3EIZzGCWf9oqP" +
            "ZmtLcgi14/jgkTFznSb0RQcHZJQThh2MtY79/T3usejRdYldh/2XaEHmwhaLwAAU" +
            "sC+edHHv4L9b40HJKF6qorMZgFyM13pk3lTl9pCxY7yEH/n8kQU1tbnG7nvGJB36" +
            "m5QSsW3Pfvy4U42tHCXBFVo=" +
            "-----END CERTIFICATE-----";

            var certificate = new X509Certificate2(Encoding.UTF8.GetBytes(certificateBody));

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
                server.ListenSecure(12360, certificate);

                using (var client = new FeatherTcpClient<BinaryMessage>()) {
                    await client.ConnectSecureAsync("localhost", 12360);
                    await client.SendAsync(TestMessage1);
                    await client.SendAsync(TestMessage2);

                    block.WaitOne(1000);
                }
            }

            Assert.Equal(2, state);
        }
    }
}
