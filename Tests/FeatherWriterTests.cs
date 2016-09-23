using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using ThreePlay.IO.Feather;
using InvertedTomato.IO.Feather.CSVCodec;
using System.Text;

namespace InvertedTomato.Feather.Tests {
    [TestClass]
    public class FeatherWriterTests {
        private CSVEncoder Payload;

        [TestInitialize]
        public void Initialize() {
            Payload = new CSVEncoder();
            Payload.WriteInteger(1);
            Payload.WriteBoolean(true);
            Payload.WriteString("test");
        }

        [TestMethod]
        public void Write_Once() {
            using (var stream = new MemoryStream()) {
                // Write to stream
                using (var writer = new FeatherWriter(stream)) {
                    writer.Write(Payload);
                }

                Assert.AreEqual("1,true,test\n", Encoding.UTF8.GetString(stream.ToArray()));
            }
        }

        [TestMethod]
        public void Write_Twice() {
            using (var stream = new MemoryStream()) {
                // Write to stream
                using (var writer = new FeatherWriter(stream)) {
                    writer.Write(Payload);
                    writer.Write(Payload);
                }

                Assert.AreEqual("1,true,test\n1,true,test\n", Encoding.UTF8.GetString(stream.ToArray()));
            }
        }

        [TestMethod]
        public void Write_Multiple() {
            using (var stream = new MemoryStream()) {
                // Write to stream
                using (var writer = new FeatherWriter(stream)) {
                    writer.Write(new[] { Payload, Payload });
                }

                Assert.AreEqual("1,true,test\n1,true,test\n", Encoding.UTF8.GetString(stream.ToArray()));
            }
        }
    }
}
