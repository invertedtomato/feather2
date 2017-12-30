using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvertedTomato.IO.Feather.ClassicCodec;
using System.Linq;
using InvertedTomato.Buffers;

namespace InvertedTomato.Feather.Tests {
    [TestClass]
    public class ClassicDecoderTests {
        public static Buffer<Byte> StringToBuffer(String hex) {
            hex = hex.Replace("-", "");
            return new Buffer<Byte>(Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray());
        }

        [TestMethod]
        public void ReadUInt8_1() {
            var encoder = new ClassicDecoder(StringToBuffer("01-00-01"));
            Assert.AreEqual(1, encoder.ReadUInt8());
        }
        [TestMethod]
        public void ReadNullableUInt8_1() {
            var encoder = new ClassicDecoder(StringToBuffer("02-00-01-01"));
            Assert.AreEqual((Byte)1, encoder.ReadNullableUInt8());
        }
        [TestMethod]
        public void ReadNullableUInt8_Null() {
            var encoder = new ClassicDecoder(StringToBuffer("01-00-00"));
            Assert.AreEqual(null, encoder.ReadNullableUInt8());
        }

        [TestMethod]
        public void ReadSInt8_1() {
            var encoder = new ClassicDecoder(StringToBuffer("01-00-01"));
            Assert.AreEqual(1, encoder.ReadSInt8());
        }
        [TestMethod]
        public void ReadSInt8_Minus1() {
            var encoder = new ClassicDecoder(StringToBuffer("01-00-FF"));
            Assert.AreEqual(-1, encoder.ReadSInt8());
        }
        [TestMethod]
        public void ReadNullableSInt8_1() {
            var encoder = new ClassicDecoder(StringToBuffer("02-00-01-01"));
            Assert.AreEqual((SByte)1, encoder.ReadNullableSInt8());
        }
        [TestMethod]
        public void ReadNullableSInt8_Null() {
            var encoder = new ClassicDecoder(StringToBuffer("01-00-00"));
            Assert.AreEqual(null, encoder.ReadNullableSInt8());
        }

        [TestMethod]
        public void ReadUInt16_1() {
            var encoder = new ClassicDecoder(StringToBuffer("02-00-01-00"));
            Assert.AreEqual(1, encoder.ReadUInt16());
        }
        [TestMethod]
        public void ReadNullableUInt16_1() {
            var encoder = new ClassicDecoder(StringToBuffer("03-00-01-01-00"));
            Assert.AreEqual((UInt16)1, encoder.ReadNullableUInt16());
        }
        [TestMethod]
        public void ReadNullableUInt16_Null() {
            var encoder = new ClassicDecoder(StringToBuffer("01-00-00"));
            Assert.AreEqual(null, encoder.ReadNullableUInt16());
        }

        [TestMethod]
        public void ReadSInt16_1() {
            var encoder = new ClassicDecoder(StringToBuffer("02-00-01-00"));
            Assert.AreEqual(1, encoder.ReadSInt16());
        }
        [TestMethod]
        public void ReadSInt16_Minus1() {
            var encoder = new ClassicDecoder(StringToBuffer("02-00-FF-FF"));
            Assert.AreEqual(-1, encoder.ReadSInt16());
        }
        [TestMethod]
        public void ReadNullableSInt16_1() {
            var encoder = new ClassicDecoder(StringToBuffer("03-00-01-01-00"));
            Assert.AreEqual((Int16)1, encoder.ReadNullableSInt16());
        }
        [TestMethod]
        public void ReadNullableSInt16_Null() {
            var encoder = new ClassicDecoder(StringToBuffer("01-00-00"));
            Assert.AreEqual(null, encoder.ReadNullableSInt16());
        }

        [TestMethod]
        public void ReadUInt32_1() {
            var encoder = new ClassicDecoder(StringToBuffer("04-00-01-00-00-00"));
            Assert.AreEqual((UInt32)1, encoder.ReadUInt32());
        }
        [TestMethod]
        public void ReadNullableUInt32_1() {
            var encoder = new ClassicDecoder(StringToBuffer("05-00-01-01-00-00-00"));
            Assert.AreEqual((UInt32)1, encoder.ReadNullableUInt32());
        }
        [TestMethod]
        public void ReadNullableUInt32_Null() {
            var encoder = new ClassicDecoder(StringToBuffer("01-00-00"));
            Assert.AreEqual(null, encoder.ReadNullableUInt32());
        }

        [TestMethod]
        public void ReadSInt32_1() {
            var encoder = new ClassicDecoder(StringToBuffer("04-00-01-00-00-00"));
            Assert.AreEqual(1, encoder.ReadSInt32());
        }
        [TestMethod]
        public void ReadSInt32_Minus1() {
            var encoder = new ClassicDecoder(StringToBuffer("04-00-FF-FF-FF-FF"));
            Assert.AreEqual(-1, encoder.ReadSInt32());
        }
        [TestMethod]
        public void ReadNullableSInt32_1() {
            var encoder = new ClassicDecoder(StringToBuffer("05-00-01-01-00-00-00"));
            Assert.AreEqual(1, encoder.ReadNullableSInt32());
        }
        [TestMethod]
        public void ReadNullableSInt32_Null() {
            var encoder = new ClassicDecoder(StringToBuffer("01-00-00"));
            Assert.AreEqual(null, encoder.ReadNullableSInt32());
        }

        [TestMethod]
        public void ReadUInt64_1() {
            var encoder = new ClassicDecoder(StringToBuffer("08-00-01-00-00-00-00-00-00-00"));
            Assert.AreEqual((UInt64)1, encoder.ReadUInt64());
        }
        [TestMethod]
        public void ReadNullableUInt64_1() {
            var encoder = new ClassicDecoder(StringToBuffer("09-00-01-01-00-00-00-00-00-00-00"));
            Assert.AreEqual((UInt64)1, encoder.ReadNullableUInt64());
        }
        [TestMethod]
        public void ReadNullableUInt64_Null() {
            var encoder = new ClassicDecoder(StringToBuffer("01-00-00"));
            Assert.AreEqual(null, encoder.ReadNullableUInt64());
        }

        [TestMethod]
        public void ReadSInt64_1() {
            var encoder = new ClassicDecoder(StringToBuffer("08-00-01-00-00-00-00-00-00-00"));
            Assert.AreEqual(1, encoder.ReadSInt64());
        }
        [TestMethod]
        public void ReadSInt64_Minus1() {
            var encoder = new ClassicDecoder(StringToBuffer("08-00-FF-FF-FF-FF-FF-FF-FF-FF"));
            Assert.AreEqual(-1, encoder.ReadSInt64());
        }
        [TestMethod]
        public void ReadNullableSInt64_1() {
            var encoder = new ClassicDecoder(StringToBuffer("09-00-01-01-00-00-00-00-00-00-00"));
            Assert.AreEqual(1, encoder.ReadNullableSInt64());
        }
        [TestMethod]
        public void ReadNullableSInt64_Null() {
            var encoder = new ClassicDecoder(StringToBuffer("01-00-00"));
            Assert.AreEqual(null, encoder.ReadNullableSInt64());
        }

        [TestMethod]
        public void ReadFloat_1() {
            var encoder = new ClassicDecoder(StringToBuffer("04-00-00-00-80-3F"));
            Assert.AreEqual(1, encoder.ReadFloat());
        }
        [TestMethod]
        public void ReadNullableFloat_Null() {
            var encoder = new ClassicDecoder(StringToBuffer("01-00-00"));
            Assert.AreEqual(null, encoder.ReadNullableFloat());
        }
        [TestMethod]
        public void ReadNullableFloat_1() {
            var encoder = new ClassicDecoder(StringToBuffer("05-00-01-00-00-80-3F"));
            Assert.AreEqual(1, encoder.ReadNullableFloat());
        }

        [TestMethod]
        public void ReadDouble_1() {
            var encoder = new ClassicDecoder(StringToBuffer("08-00-00-00-00-00-00-00-F0-3F"));
            Assert.AreEqual(1, encoder.ReadDouble());
        }
        [TestMethod]
        public void ReadNullableDouble_Null() {
            var encoder = new ClassicDecoder(StringToBuffer("01-00-00"));
            Assert.AreEqual(null, encoder.ReadNullableDouble());
        }
        [TestMethod]
        public void ReadNullableDouble_1() {
            var encoder = new ClassicDecoder(StringToBuffer("09-00-01-00-00-00-00-00-00-F0-3F"));
            Assert.AreEqual(1, encoder.ReadNullableDouble());
        }

        [TestMethod]
        public void ReadBoolean_True() {
            var encoder = new ClassicDecoder(StringToBuffer("01-00-FF"));
            Assert.AreEqual(true, encoder.ReadBoolean());
        }
        [TestMethod]
        public void ReadBoolean_False() {
            var encoder = new ClassicDecoder(StringToBuffer("01-00-00"));
            Assert.AreEqual(false, encoder.ReadBoolean());
        }

        [TestMethod]
        public void ReadGuid_Random() {
            var value = Guid.NewGuid();
            var encoder = new ClassicDecoder(new Buffer<Byte>(new Byte[] { 16, 0 }.Concat(value.ToByteArray()).ToArray()));
            Assert.AreEqual(value, encoder.ReadGuid());
        }
        [TestMethod]
        public void ReadNullableGuid_Random() {
            var value = Guid.NewGuid();
            var encoder = new ClassicDecoder(new Buffer<Byte>(new Byte[] { 17, 0, 1 }.Concat(value.ToByteArray()).ToArray()));
            Assert.AreEqual(value, encoder.ReadNullableGuid());
        }
        [TestMethod]
        public void ReadNullableGuid_Null() {
            var encoder = new ClassicDecoder(StringToBuffer("01-00-00"));
            Assert.AreEqual(null, encoder.ReadNullableGuid());
        }

        [TestMethod]
        public void ReadTime_1() {
            var encoder = new ClassicDecoder(StringToBuffer("08-00-01-00-00-00-00-00-00-00"));
            Assert.AreEqual(new TimeSpan(1), encoder.ReadTime());
        }
        [TestMethod]
        public void ReadNullableTime_1() {
            var encoder = new ClassicDecoder(StringToBuffer("09-00-01-01-00-00-00-00-00-00-00"));
            Assert.AreEqual(new TimeSpan(1), encoder.ReadNullableTime());
        }
        [TestMethod]
        public void ReadNullableTime_Nullable() {
            var encoder = new ClassicDecoder(StringToBuffer("01-00-00"));
            Assert.AreEqual(null, encoder.ReadNullableTime());
        }

        [TestMethod]
        public void ReadTimeSimple_1() {
            var encoder = new ClassicDecoder(StringToBuffer("08-00-01-00-00-00-00-00-00-00"));
            Assert.AreEqual(new TimeSpan(0, 0, 1), encoder.ReadTimeSimple());
        }
        [TestMethod]
        public void ReadNullableTimeSimple_1() {
            var encoder = new ClassicDecoder(StringToBuffer("09-00-01-01-00-00-00-00-00-00-00"));
            Assert.AreEqual(new TimeSpan(0, 0, 1), encoder.ReadNullableTimeSimple());
        }
        [TestMethod]
        public void ReadNullableTimeSimple_Nullable() {
            var encoder = new ClassicDecoder(StringToBuffer("01-00-00"));
            Assert.AreEqual(null, encoder.ReadNullableTimeSimple());
        }

        [TestMethod]
        public void ReadDateTime_1() {
            var encoder = new ClassicDecoder(StringToBuffer("08-00-01-00-00-00-00-00-00-00"));
            Assert.AreEqual(new DateTime(1), encoder.ReadDateTime());
        }
        [TestMethod]
        public void ReadNullableDateTime_1() {
            var encoder = new ClassicDecoder(StringToBuffer("09-00-01-01-00-00-00-00-00-00-00"));
            Assert.AreEqual(new DateTime(1), encoder.ReadNullableDateTime());
        }
        [TestMethod]
        public void ReadNullableDateTime_Nullable() {
            var encoder = new ClassicDecoder(StringToBuffer("01-00-00"));
            Assert.AreEqual(null, encoder.ReadNullableDateTime());
        }

        [TestMethod]
        public void ReadDateTimeSimple_1() {
            var encoder = new ClassicDecoder(StringToBuffer("08-00-01-00-00-00-00-00-00-00"));
            Assert.AreEqual(DateUtility.Epoch.AddSeconds(1), encoder.ReadDateTimeSimple());
        }
        [TestMethod]
        public void ReadNullableDateTimeSimple_1() {
            var encoder = new ClassicDecoder(StringToBuffer("09-00-01-01-00-00-00-00-00-00-00"));
            Assert.AreEqual(DateUtility.Epoch.AddSeconds(1), encoder.ReadNullableDateTimeSimple());
        }
        [TestMethod]
        public void ReadNullableDateTimeSimple_Nullable() {
            var encoder = new ClassicDecoder(StringToBuffer("01-00-00"));
            Assert.AreEqual(null, encoder.ReadNullableDateTimeSimple());
        }

        [TestMethod]
        public void ReadIPAddress_Test() {
            var encoder = new ClassicDecoder(StringToBuffer("05-00-04-01-02-03-04"));
            Assert.AreEqual("1.2.3.4", encoder.ReadIPAddress().ToString());
        }
        [TestMethod]
        public void ReadNullableIPAddress_Test() {
            var encoder = new ClassicDecoder(StringToBuffer("05-00-04-01-02-03-04"));
            Assert.AreEqual("1.2.3.4", encoder.ReadNullableIPAddress().ToString());
        }
        [TestMethod]
        public void ReadNullableIPAddress_Null() {
            var encoder = new ClassicDecoder(StringToBuffer("01-00-00"));
            Assert.AreEqual(null, encoder.ReadNullableIPAddress());
        }

        [TestMethod]
        public void ReadString_Test() {
            var encoder = new ClassicDecoder(StringToBuffer("06-00-04-00-74-65-73-74"));
           Assert.AreEqual("test", encoder.ReadString());
        }
        [TestMethod]
        public void ReadNullableString_Test() {
            var encoder = new ClassicDecoder(StringToBuffer("07-00-01-04-00-74-65-73-74"));
            Assert.AreEqual("test", encoder.ReadNullableString());
        }
        [TestMethod]
        public void ReadNullableString_Null() {
            var encoder = new ClassicDecoder(StringToBuffer("01-00-00"));
            Assert.AreEqual(null, encoder.ReadNullableString());
        }

        [TestMethod]
        public void Read_Raw() {
            var encoder = new ClassicDecoder(StringToBuffer("03-00-01-02-03"));
            Assert.AreEqual("01-02-03", BitConverter.ToString(encoder.Read(3)));
        }
    }
}
