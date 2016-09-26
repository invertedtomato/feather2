using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvertedTomato.IO.Feather.ClassicCodec;

namespace InvertedTomato.Feather.Tests {
    [TestClass]
    public class ClassicEncoderTests {
        [TestMethod]
        public void WriteUInt8_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteUInt8(1);
            Assert.AreEqual("01-00-01", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableUInt8_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableUInt8(1);
            Assert.AreEqual("02-00-01-01", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableUInt8_Null() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableUInt8(null);
            Assert.AreEqual("01-00-00", encoder.GetBuffer().ToString());
        }

        [TestMethod]
        public void WriteSInt8_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteSInt8(1);
            Assert.AreEqual("01-00-01", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteSInt8_Minus1() {
            var encoder = new ClassicEncoder();
            encoder.WriteSInt8(-1);
            Assert.AreEqual("01-00-FF", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableSInt8_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableSInt8(1);
            Assert.AreEqual("02-00-01-01", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableSInt8_Null() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableSInt8(null);
            Assert.AreEqual("01-00-00", encoder.GetBuffer().ToString());
        }

        [TestMethod]
        public void WriteUInt16_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteUInt16(1);
            Assert.AreEqual("02-00-01-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableUInt16_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableUInt16(1);
            Assert.AreEqual("03-00-01-01-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableUInt16_Null() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableUInt16(null);
            Assert.AreEqual("01-00-00", encoder.GetBuffer().ToString());
        }

        [TestMethod]
        public void WriteSInt16_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteSInt16(1);
            Assert.AreEqual("02-00-01-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteSInt16_Minus1() {
            var encoder = new ClassicEncoder();
            encoder.WriteSInt16(-1);
            Assert.AreEqual("02-00-FF-FF", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableSInt16_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableSInt16(1);
            Assert.AreEqual("03-00-01-01-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableSInt16_Null() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableSInt16(null);
            Assert.AreEqual("01-00-00", encoder.GetBuffer().ToString());
        }

        [TestMethod]
        public void WriteUInt32_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteUInt32(1);
            Assert.AreEqual("04-00-01-00-00-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableUInt32_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableUInt32(1);
            Assert.AreEqual("05-00-01-01-00-00-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableUInt32_Null() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableUInt32(null);
            Assert.AreEqual("01-00-00", encoder.GetBuffer().ToString());
        }

        [TestMethod]
        public void WriteSInt32_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteSInt32(1);
            Assert.AreEqual("04-00-01-00-00-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteSInt32_Minus1() {
            var encoder = new ClassicEncoder();
            encoder.WriteSInt32(-1);
            Assert.AreEqual("04-00-FF-FF-FF-FF", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableSInt32_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableSInt32(1);
            Assert.AreEqual("05-00-01-01-00-00-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableSInt32_Null() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableSInt32(null);
            Assert.AreEqual("01-00-00", encoder.GetBuffer().ToString());
        }
    }
}
