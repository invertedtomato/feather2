using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using InvertedTomato.Buffers;
using InvertedTomato.IO.Feather.CSVCodec;

namespace InvertedTomato.Feather.Tests {
    [TestClass]
    public class CSVDecoderTests {
        public string[][] DecodeAllSymbolSets(string value) {
            // Create buffer
            var buffer = new Buffer<byte>(Encoding.UTF8.GetBytes(value));

            // Call decode until nothing more is returned
            var symbolSets = new List<string[]>();
            Buffer<string> symbols;
            while (null != (symbols = CSVDecoder.Decode(buffer))) {
                symbolSets.Add(symbols.ToArray());
            }

            // Return as array
            return symbolSets.ToArray();
        }
        public string[] DecodeOnlySymbolSet(string value) {
            var symbolSets = DecodeAllSymbolSets(value);
            Assert.AreEqual(1, symbolSets.Length);
            return symbolSets[0];
        }
        public string DecodeOnlySymbol(string value) {
            var symbols = DecodeOnlySymbolSet(value);
            Assert.AreEqual(1, symbols.Length);
            return symbols[0];
        }
        

        [TestMethod]
        public void Decode_Complete() {
            var buffer = new Buffer<byte>(Encoding.UTF8.GetBytes("a\n"));
            Assert.AreEqual(0, buffer.Start);
            Assert.AreEqual(2, buffer.Used);

            Assert.IsFalse(null == CSVDecoder.Decode(buffer));
            Assert.AreEqual(2, buffer.Start);
            Assert.AreEqual(0, buffer.Used);

            Assert.IsTrue(null == CSVDecoder.Decode(buffer));
            Assert.AreEqual(2, buffer.Start);
            Assert.AreEqual(0, buffer.Used);
        }
        [TestMethod]
        public void Decode_1() {
            Assert.AreEqual("1", DecodeOnlySymbol("1\n"));
        }
        [TestMethod]
        public void Decode_a() {
            Assert.AreEqual("a", DecodeOnlySymbol("a\n"));
        }
        [TestMethod]
        public void Decode_Blank() {
            Assert.AreEqual("", DecodeOnlySymbol("\n"));
        }
        [TestMethod]
        public void Decode_Comma() {
            Assert.AreEqual(",", DecodeOnlySymbol("\",\"\n"));
        }
        [TestMethod]
        public void Decode_Quote() {
            Assert.AreEqual("\"", DecodeOnlySymbol("\"\\\"\"\n"));
        }
        [TestMethod]
        public void Decode_a_b_c() {
            var result = DecodeOnlySymbolSet("a,b,c\n");
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("a", result[0]);
            Assert.AreEqual("b", result[1]);
            Assert.AreEqual("c", result[2]);
        }
        [TestMethod]
        public void Decode_Complex() {
            var result = DecodeOnlySymbolSet("1,\"Don't talk to \\\"Jill\\\"\\\\\\\"Tom\\\"!.\",b,\n");
            Assert.AreEqual(4, result.Length);
            Assert.AreEqual("1", result[0]);
            Assert.AreEqual("Don't talk to \"Jill\"\\\"Tom\"!.", result[1]);
            Assert.AreEqual("b", result[2]);
            Assert.AreEqual("", result[3]);
        }

        [TestMethod]
        public void ReadInteger_10() {
            var reader = new CSVDecoder(new string[] { "10" });
            Assert.AreEqual(10, reader.ReadInteger());
        }
        [TestMethod]
        public void ReadInteger_10_10() {
            var reader = new CSVDecoder(new string[] { "10", "10" });
            Assert.AreEqual(10, reader.ReadInteger());
            Assert.AreEqual(10, reader.ReadInteger());
        }

        [TestMethod]
        public void ReadDecimal_10Point1() {
            var reader = new CSVDecoder(new string[] { "10.1" });
            Assert.AreEqual((decimal)10.1, reader.ReadDecimal());
        }

        [TestMethod]
        public void ReadBoolean_True() {
            var reader = new CSVDecoder(new string[] { "true" });
            Assert.AreEqual(true, reader.ReadBoolean());
        }
        [TestMethod]
        public void ReadBoolean_False() {
            var reader = new CSVDecoder(new string[] { "false" });
            Assert.AreEqual(false, reader.ReadBoolean());
        }

        [TestMethod]
        public void ReadDateTime_1() {
            var reader = new CSVDecoder(new string[] { "2000-01-01 01:02:03Z" });
            Assert.AreEqual(new DateTime(2000, 1, 1, 1, 2, 3, DateTimeKind.Utc), reader.ReadDateTime().ToUniversalTime());
        }

        [TestMethod]
        public void ReadTime_1() {
            var reader = new CSVDecoder(new string[] { "00:00:01" });
            Assert.AreEqual(new TimeSpan(0, 0, 1), reader.ReadTime());
        }

        [TestMethod]
        public void ReadString_Basic() {
            var reader = new CSVDecoder(new string[] { "test" });
            Assert.AreEqual("test", reader.ReadString());
        }
        [TestMethod]
        public void ReadString_Commas() {
            var reader = new CSVDecoder(new string[] { "a,b,c" });
            Assert.AreEqual("a,b,c", reader.ReadString());
        }
        [TestMethod]
        public void ReadString_Newline() {
            var reader = new CSVDecoder(new string[] { "a\nb" });
            Assert.AreEqual("a\nb", reader.ReadString());
        }
        [TestMethod]
        public void ReadString_Empty() {
            var reader = new CSVDecoder(new string[] { "" });
            Assert.AreEqual("", reader.ReadString());
        }
    }
}
