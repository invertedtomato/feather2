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
            var message = new TrivialEncoder(new byte[] { 0x01 });
            var options = new ConnectionOptions() { NoDelay = true };

            var onConnects = 0;
            var onDisconnects = 0;
            var onMessage = 0;
            var onRemoteMessages = 0;
            var onRemoteDisconnects = 0;

            var feather = new FeatherTCP<TrivialEncoder, TrivialDecoder>(options);

            // TODO: More tests!!

            // Setup event handlers
            feather.OnConnection += (rmt) => {
                onConnects++;
            };
            feather.OnDisconnection += (Remote<TrivialEncoder, TrivialDecoder> rmt, DisconnectionType reason) => {
                // Check remote
                Assert.IsTrue(rmt.RemoteEndPoint.ToString().Contains("127.0.0.1"));

                // Check disconnect reason
                Assert.AreEqual(DisconnectionType.RemoteDisconnection, reason);

                onDisconnects++;
            };
            feather.OnMessage += (Remote<TrivialEncoder, TrivialDecoder> rmt, TrivialDecoder msg) => {
                // Check remote
                Assert.IsTrue(rmt.RemoteEndPoint.ToString().Contains("127.0.0.1"));

                // Check message
                Assert.AreEqual(1, msg.Symbols.Length);
                Assert.AreEqual(0x01, msg.Symbols[0]);
                
                onMessage++;
            };

            // Listen for incoming connections
            feather.Listen(1234);
            Thread.Sleep(50);
            Assert.AreEqual(0, onConnects);
            Assert.AreEqual(0, onDisconnects);
            Assert.AreEqual(0, onMessage);

            // Make outbound connection
            var remote = feather.Connect("localhost", 1234);
            Thread.Sleep(50);
            Assert.AreEqual(1, onConnects);
            Assert.AreEqual(0, onDisconnects);
            Assert.AreEqual(0, onMessage);

            // Setup remote events
            remote.OnDisconnection = (reason) => {
                // Check disconnect reason
                Assert.AreEqual(DisconnectionType.LocalDisconnection, reason);

                onRemoteDisconnects++;
            };
            remote.OnMessage = (msg) => {
                // Check message
                Assert.AreEqual(1, msg.Symbols.Length);
                Assert.AreEqual(0x01, msg.Symbols[0]);

                onRemoteMessages++;
            };

            // Send inbound message
            remote.Send(message);
            Thread.Sleep(50);
            Assert.AreEqual(1, onConnects);
            Assert.AreEqual(0, onDisconnects);
            Assert.AreEqual(1, onMessage);
            
            // Send outbound message
            feather.Broadcast(message);
            Thread.Sleep(50);
            Assert.AreEqual(1, onConnects);
            Assert.AreEqual(0, onDisconnects);
            Assert.AreEqual(2, onMessage);
            Assert.AreEqual(1, onRemoteMessages);
            Assert.AreEqual(0, onRemoteDisconnects);

            // Disconnect
            remote.Disconnect();
            Thread.Sleep(50);
            Assert.AreEqual(1, onConnects);
            Assert.AreEqual(1, onDisconnects);
            Assert.AreEqual(2, onMessage);
            Assert.AreEqual(true, remote.IsDisposed);
            Assert.AreEqual(1, onRemoteMessages);
            Assert.AreEqual(0, onRemoteDisconnects);

            // Dispose
            feather.Dispose();
            Assert.AreEqual(true, feather.IsDisposed);
        }
    }
}
