using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using ThreePlay.IO.Feather;
using InvertedTomato.IO.Feather.CSVCodec;
using InvertedTomato.Buffers;

namespace InvertedTomato.Feather.Tests {
    [TestClass]
    public class FeatherReaderTests {
        [TestMethod]
        public void Read_None() {
            using (var stream = new MemoryStream()) {
                using (var reader = new FeatherReader(stream)) {
                    // First read (end of file already hit)
                    Assert.AreEqual(null, reader.Read<CSVDecoder>());
                }
            }
        }

        [TestMethod]
        public void Read_Once() {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("1,true,test\n"))) {
                using (var reader = new FeatherReader(stream)) {
                    // First read
                    var payload = reader.Read<CSVDecoder>();
                    Assert.AreEqual(1, payload.ReadInteger());
                    Assert.AreEqual(true, payload.ReadBoolean());
                    Assert.AreEqual("test", payload.ReadString());

                    // Second read (end of file already hit)
                    Assert.AreEqual(null, reader.Read<CSVDecoder>());
                }
            }
        }

        [TestMethod]
        public void Read_Twice() {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("1,true,test\n1,true,test\n"))) {
                using (var reader = new FeatherReader(stream)) {
                    // First read
                    var payload = reader.Read<CSVDecoder>();
                    Assert.AreEqual(1, payload.ReadInteger());
                    Assert.AreEqual(true, payload.ReadBoolean());
                    Assert.AreEqual("test", payload.ReadString());

                    // Second read
                    payload = reader.Read<CSVDecoder>();
                    Assert.AreEqual(1, payload.ReadInteger());
                    Assert.AreEqual(true, payload.ReadBoolean());
                    Assert.AreEqual("test", payload.ReadString());

                    // Third read (end of file already hit)
                    Assert.AreEqual(null, reader.Read<CSVDecoder>());
                }
            }
        }

        [TestMethod]
        public void Read_BufferGrowing() {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("1,true,test\n"))) {
                using (var reader = new FeatherReader(stream, new Options() { PayloadInitialBufferSize = 1 })) {
                    // First read
                    var payload = reader.Read<CSVDecoder>();
                    Assert.AreEqual(1, payload.ReadInteger());
                    Assert.AreEqual(true, payload.ReadBoolean());
                    Assert.AreEqual("test", payload.ReadString());

                    // Second read (end of file already hit)
                    Assert.AreEqual(null, reader.Read<CSVDecoder>());
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(MalformedPayloadException))]
        public void Read_ExceedPayloadMaxLength() {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("1,b\n"))) {
                using (var reader = new FeatherReader(stream, new Options() { PayloadMaxSize = 1 })) {
                    // First read
                    reader.Read<CSVDecoder>();
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(MalformedPayloadException))]
        public void Read_CorruptTruncaded() {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("a"))) {
                using (var reader = new FeatherReader(stream)) {
                    // First read (end of file already hit)
                    Assert.AreEqual(null, reader.Read<CSVDecoder>());
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(MalformedPayloadException))]
        public void Read_ExceedHeaderMaxLength() {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("1,true,test\n"))) {
                using (var reader = new FeatherReader(stream, new Options() { PayloadInitialBufferSize = 1 })) {
                    reader.Read<FakeDecoder>();
                }
            }
        }
    }


    public class FakeDecoder : IDecoder {
        public int MaxHeaderLength { get { return 1; } }

        public int GetPayloadLength(ReadOnlyBuffer<byte> buffer) {
            return -1;
        }

        public bool LoadBuffer(Buffer<byte> buffer) {
            throw new NotImplementedException();
        }
    }
}
