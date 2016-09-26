using System;
using InvertedTomato.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThreePlay.IO.Feather;
using InvertedTomato.Testable.Sockets;
using InvertedTomato.Net.Feather;
using System.Net;
using System.Collections.Generic;
using InvertedTomato.Feather.TrivialCodec;

namespace InvertedTomato.Feather.Tests {
    [TestClass]
    public class ConnectionBaseTests {
        [TestMethod]
        public void Send_9Byte() {
            using (var connection = new FakeConnection()) {
                Assert.AreEqual("08-01-02-03-04-05-06-07-08", BitConverter.ToString(connection.TestSend(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 })));
            }
        }
        [TestMethod]
        public void Send_1Byte() {
            using (var connection = new FakeConnection()) {
                Assert.AreEqual("00", BitConverter.ToString(connection.TestSend(new byte[] { })));
            }
        }

        [TestMethod]
        public void SendMany() {
            using (var connection = new FakeConnection()) {
                var payloads = new List<TrivialEncoder>();
                payloads.Add(new TrivialEncoder( new byte[] { 1 }));
                payloads.Add(new TrivialEncoder( new byte[] { 2 }));
                payloads.Add(new TrivialEncoder( new byte[] { 3 }));
                
                Assert.AreEqual("01-01-01-02-01-03", BitConverter.ToString(connection.TestSendMany(payloads.ToArray())));
            }
        }

        [TestMethod]
        public void Receive() {
            using (var connection = new FakeConnection()) {
                Assert.AreEqual("01", BitConverter.ToString(connection.TestReceive(new byte[] { 1, 1 })));
                Assert.AreEqual("02", BitConverter.ToString(connection.TestReceive(new byte[] { 1, 2 })));
                Assert.AreEqual("01-02-03-04-05-06-07-08", BitConverter.ToString(connection.TestReceive(new byte[] { 8, 1, 2, 3, 4, 5, 6, 7, 8 })));
            }
        }

        class FakeConnection : ConnectionBase<TrivialEncoder, TrivialDecoder> {
            public readonly SocketFake Socket = new SocketFake();
            public byte[] LastPayload;

            public FakeConnection() : this(new ConnectionOptions()) { }
            public FakeConnection(ConnectionOptions configuration) {
                if (null == configuration) {
                    throw new ArgumentNullException("configuration");
                }

                Start(false, Socket, configuration);
            }

            public byte[] TestSend(byte[] payload) {
                Send(new TrivialEncoder(payload));

                return Socket.Stream.ReadOutput();
            }
            public byte[] TestSendMany(TrivialEncoder[] payloads) {
                Send(payloads);

                return Socket.Stream.ReadOutput();
            }

            public byte[] TestReceive(byte[] wire) {
                Socket.Stream.QueueInput(wire);

                return LastPayload;
            }

            protected override void OnDataArrived(TrivialDecoder payload) {
                LastPayload = payload.SymbolBuffer.ToArray();
            }
        }
    }
}
