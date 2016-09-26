using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvertedTomato.Feather.TrivialCodec;
using InvertedTomato.Net.Feather;
using System.Net;
using System.Threading;

namespace InvertedTomato.Feather.Tests {
    [TestClass]
    public class FeatherTCPTests {
        [TestMethod]
        public void EndToEnd() {
            var clientConnected = 0;
            var clientDisconnected = 0;
            var clientPings = 0;
            var serverPings = 0;
            var options = new ConnectionOptions() {
                NoDelay = true
            };

            // Create server
            using (var server = FeatherTCP<RealConnection,TrivialEncoder,TrivialDecoder>.Listen(1234, options)) {
                server.OnClientConnected += (connection) => {
                    clientConnected++;
                    connection.OnPingReceived += () => { serverPings++; };
                    connection.OnDisconnected += (reason) => { clientDisconnected++; };

                    // Send pings
                    connection.SendPing();
                    connection.SendPing();
                    connection.SendPing();
                };

                // Create client
                using (var client = FeatherTCP<RealConnection, TrivialEncoder, TrivialDecoder>.Connect("localhost", 1234, options)) {
                    client.OnPingReceived += () => { clientPings++; };

                    // Send pings
                    client.SendPing();
                    client.SendPing();

                    // Wait for sending to complete
                    Thread.Sleep(50);

                    // Dispose and check
                    client.Dispose();
                    Assert.IsTrue(client.IsDisposed);
                }

                // Wait for sending to complete
                Thread.Sleep(50);

                // Dispose and check
                server.Dispose();
                Assert.IsTrue(server.IsDisposed);
            }

            // Check all worked
            Assert.AreEqual(1, clientConnected);
            Assert.AreEqual(1, clientDisconnected);
            Assert.AreEqual(2, serverPings);
            Assert.AreEqual(3, clientPings);
        }

        class RealConnection : ConnectionBase<TrivialEncoder, TrivialDecoder> {
            public Action OnPingReceived;

            public void SendPing() {
                Send(new TrivialEncoder(new byte[] { 0x01 }));
            }

            protected override void OnMessageReceived(TrivialDecoder payload) {
                if (payload.SymbolBuffer.Used != 2) {
                    throw new ProtocolViolationException("Unexpected length");
                }
                if (payload.SymbolBuffer.Dequeue() != 0x01) {
                    throw new ProtocolViolationException("Unexpected opcode.");
                }
                if (payload.SymbolBuffer.Dequeue() != 0x01) {
                    throw new ProtocolViolationException("Unexpected opcode.");
                }

                OnPingReceived.TryInvoke();
            }
        }
    }
}
