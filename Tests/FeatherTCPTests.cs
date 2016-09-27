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

            var feather = new FeatherTCP<TrivialEncoder, TrivialDecoder>(options);

            // Setup event handlers
            feather.OnConnection += (rmt) => {
                onConnects++;
            };
            feather.OnDisconnection += (rmt, reason) => {
                // TODO: check rmt
                // TODO: check reason

                onDisconnects++;
            };
            feather.OnMessage += (Remote<TrivialEncoder, TrivialDecoder> rmt, TrivialDecoder msg) => {
                // TODO: check rmt

                // Check message
                Assert.AreEqual(1, msg.Symbols.Length);
                Assert.AreEqual(0x01, msg.Symbols[0]);

                // TODO: check params
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
            Assert.AreEqual(3, onMessage);

            // Disconnect
            remote.Disconnect();
            Thread.Sleep(50);
            Assert.AreEqual(1, onConnects);
            Assert.AreEqual(2, onDisconnects);
            Assert.AreEqual(3, onMessage);
            Assert.AreEqual(true, remote.IsDisposed);

            // Dispose
            feather.Dispose();
            Assert.AreEqual(true, feather.IsDisposed);

            // TODO: Testing of Remote events
        }
    }
}
