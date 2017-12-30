using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using InvertedTomato.IO.Feather.CSVCodec;

namespace InvertedTomato.Feather.Tests {
    [TestClass]
    public class CSVEncoderTests {
        public String EncodeSymbolSet(String[] symbolSet) {
            var buffer = CSVEncoder.Encode(new List<String>(symbolSet));
            return Encoding.UTF8.GetString(buffer.ToArray());
        }

        [TestMethod]
        public void Encode_1() {
            Assert.AreEqual("1\n", EncodeSymbolSet(new String[] { "1" }));
        }
        [TestMethod]
        public void Encode_a() {
            Assert.AreEqual("a\n", EncodeSymbolSet(new String[] { "a" }));
        }
        [TestMethod]
        public void Encode_Blank() {
            Assert.AreEqual("\n", EncodeSymbolSet(new String[] { }));
        }
        [TestMethod]
        public void Encode_Comma() {
            Assert.AreEqual("\",\"\n", EncodeSymbolSet(new String[] { "," }));
        }
        [TestMethod]
        public void Encode_Quote() {
            Assert.AreEqual("\"\\\"\"\n", EncodeSymbolSet(new String[] { "\"" }));
        }
        [TestMethod]
        public void Encode_Unicode() {
            Assert.AreEqual("\x10A7\n", EncodeSymbolSet(new String[] { "\x10A7" }));
        }
        [TestMethod]
        public void Encode_a_b_c() {
            Assert.AreEqual("a,b,c\n", EncodeSymbolSet(new String[] { "a", "b", "c" }));
        }
        [TestMethod]
        public void Encode_Complex() {
            Assert.AreEqual("1,\"Don't talk to \\\"Jill\\\"\\\\\\\"Tom\\\"!.\",\x10A7,\n", EncodeSymbolSet(new String[] { "1", "Don't talk to \"Jill\"\\\"Tom\"!.", "\x10A7", "" }));
        }


        [TestMethod]
        public void WriteInteger_10() {
            var writer = new CSVEncoder();
            writer.WriteInteger(10);
            Assert.AreEqual(1, writer.Symbols.Count);
            Assert.AreEqual("10", writer.Symbols[0]);
        }
        [TestMethod]
        public void WriteInteger_10_10() {
            var writer = new CSVEncoder();
            writer.WriteInteger(10);
            writer.WriteInteger(10);
            Assert.AreEqual(2, writer.Symbols.Count);
            Assert.AreEqual("10", writer.Symbols[0]);
            Assert.AreEqual("10", writer.Symbols[1]);
        }
        [TestMethod]
        public void WriteInteger_Minus10() {
            var writer = new CSVEncoder();
            writer.WriteInteger(-10);
            Assert.AreEqual(1, writer.Symbols.Count);
            Assert.AreEqual("-10", writer.Symbols[0]);
        }
        [TestMethod]
        public void WriteDecimal_10Point1() {
            var writer = new CSVEncoder();
            writer.WriteDecimal((Decimal)10.1);
            Assert.AreEqual(1, writer.Symbols.Count);
            Assert.AreEqual("10.1", writer.Symbols[0]);
        }
        [TestMethod]
        public void WriteBoolean_True() {
            var writer = new CSVEncoder();
            writer.WriteBoolean(true);
            Assert.AreEqual(1, writer.Symbols.Count);
            Assert.AreEqual("true", writer.Symbols[0]);
        }
        [TestMethod]
        public void WriteBoolean_False() {
            var writer = new CSVEncoder();
            writer.WriteBoolean(false);
            Assert.AreEqual(1, writer.Symbols.Count);
            Assert.AreEqual("false", writer.Symbols[0]);
        }
        [TestMethod]
        public void WriteDateTime_1() {
            var writer = new CSVEncoder();
            writer.WriteDateTime(new DateTime(2000, 1, 1, 1, 2, 3));
            Assert.AreEqual(1, writer.Symbols.Count);
            Assert.AreEqual("2000-01-01 01:02:03Z", writer.Symbols[0]);
        }
        [TestMethod]
        public void WriteTime_1() {
            var writer = new CSVEncoder();
            writer.WriteTime(new TimeSpan(0, 0, 1));
            Assert.AreEqual(1, writer.Symbols.Count);
            Assert.AreEqual("00:00:01", writer.Symbols[0]);
        }
        [TestMethod]
        public void WriteString_Basic() {
            var writer = new CSVEncoder();
            writer.WriteString("test");
            Assert.AreEqual(1, writer.Symbols.Count);
            Assert.AreEqual("test", writer.Symbols[0]);
        }
        [TestMethod]
        public void WriteString_Commas() {
            var writer = new CSVEncoder();
            writer.WriteString("a,b,c");
            Assert.AreEqual(1, writer.Symbols.Count);
            Assert.AreEqual("a,b,c", writer.Symbols[0]);
        }
        [TestMethod]
        public void WriteString_Newline() {
            var writer = new CSVEncoder();
            writer.WriteString("a\nb");
            Assert.AreEqual(1, writer.Symbols.Count);
            Assert.AreEqual("a\nb", writer.Symbols[0]);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteString_Null() {
            var writer = new CSVEncoder();
            writer.WriteString(null);
        }
    }
}
