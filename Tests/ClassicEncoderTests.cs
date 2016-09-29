using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvertedTomato.IO.Feather.ClassicCodec;
using System.Linq;
using System.IO;
using ThreePlay.IO.Feather;
using System.Net;

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

        [TestMethod]
        public void WriteUInt64_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteUInt64(1);
            Assert.AreEqual("08-00-01-00-00-00-00-00-00-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableUInt64_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableUInt64(1);
            Assert.AreEqual("09-00-01-01-00-00-00-00-00-00-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableUInt64_Null() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableUInt64(null);
            Assert.AreEqual("01-00-00", encoder.GetBuffer().ToString());
        }

        [TestMethod]
        public void WriteSInt64_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteSInt64(1);
            Assert.AreEqual("08-00-01-00-00-00-00-00-00-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteSInt64_Minus1() {
            var encoder = new ClassicEncoder();
            encoder.WriteSInt64(-1);
            Assert.AreEqual("08-00-FF-FF-FF-FF-FF-FF-FF-FF", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableSInt64_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableSInt64(1);
            Assert.AreEqual("09-00-01-01-00-00-00-00-00-00-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableSInt64_Null() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableSInt64(null);
            Assert.AreEqual("01-00-00", encoder.GetBuffer().ToString());
        }

        [TestMethod]
        public void WriteFloat_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteFloat(1);
            Assert.AreEqual("04-00-00-00-80-3F", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableFloat_Null() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableFloat(null);
            Assert.AreEqual("01-00-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableFloat_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableFloat(1);
            Assert.AreEqual("05-00-01-00-00-80-3F", encoder.GetBuffer().ToString());
        }

        [TestMethod]
        public void WriteDouble_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteDouble(1);
            Assert.AreEqual("08-00-00-00-00-00-00-00-F0-3F", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableDouble_Null() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableDouble(null);
            Assert.AreEqual("01-00-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableDouble_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableDouble(1);
            Assert.AreEqual("09-00-01-00-00-00-00-00-00-F0-3F", encoder.GetBuffer().ToString());
        }

        [TestMethod]
        public void WriteBoolean_True() {
            var encoder = new ClassicEncoder();
            encoder.WriteBoolean(true);
            Assert.AreEqual("01-00-01", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteBoolean_False() {
            var encoder = new ClassicEncoder();
            encoder.WriteBoolean(false);
            Assert.AreEqual("01-00-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableBoolean_True() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableBoolean(true);
            Assert.AreEqual("02-00-01-01", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableBoolean_False() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableBoolean(false);
            Assert.AreEqual("02-00-01-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableBoolean_Null() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableBoolean(null);
            Assert.AreEqual("01-00-00", encoder.GetBuffer().ToString());
        }

        [TestMethod]
        public void WriteGuid_Random() {
            var value = Guid.NewGuid();

            var encoder = new ClassicEncoder();
            encoder.WriteGuid(value);
            Assert.AreEqual(BitConverter.ToString(new byte[] { 16, 0 }.Concat(value.ToByteArray()).ToArray()), encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableGuid_Random() {
            var value = Guid.NewGuid();

            var encoder = new ClassicEncoder();
            encoder.WriteNullableGuid(value);
            Assert.AreEqual(BitConverter.ToString(new byte[] { 17, 0, 1 }.Concat(value.ToByteArray()).ToArray()), encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableGuid_Null() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableGuid(null);
            Assert.AreEqual("01-00-00", encoder.GetBuffer().ToString());
        }

        [TestMethod]
        public void WriteTime_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteTime(new TimeSpan(1));
            Assert.AreEqual("08-00-01-00-00-00-00-00-00-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableTime_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableTime(new TimeSpan(1));
            Assert.AreEqual("09-00-01-01-00-00-00-00-00-00-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableTime_Nullable() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableTime(null);
            Assert.AreEqual("01-00-00", encoder.GetBuffer().ToString());
        }

        [TestMethod]
        public void WriteTimeSimple_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteTimeSimple(new TimeSpan(0, 0, 1));
            Assert.AreEqual("08-00-01-00-00-00-00-00-00-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableTimeSimple_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableTimeSimple(new TimeSpan(0, 0, 1));
            Assert.AreEqual("09-00-01-01-00-00-00-00-00-00-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableTimeSimple_Nullable() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableTimeSimple(null);
            Assert.AreEqual("01-00-00", encoder.GetBuffer().ToString());
        }

        [TestMethod]
        public void WriteDateTime_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteDateTime(new DateTime(1));
            Assert.AreEqual("08-00-01-00-00-00-00-00-00-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableDateTime_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableDateTime(new DateTime(1));
            Assert.AreEqual("09-00-01-01-00-00-00-00-00-00-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableDateTime_Nullable() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableDateTime(null);
            Assert.AreEqual("01-00-00", encoder.GetBuffer().ToString());
        }

        [TestMethod]
        public void WriteDateTimeSimple_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteDateTimeSimple(DateUtility.Epoch.AddSeconds(1));
            Assert.AreEqual("08-00-01-00-00-00-00-00-00-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableDateTimeSimple_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableDateTimeSimple(DateUtility.Epoch.AddSeconds(1));
            Assert.AreEqual("09-00-01-01-00-00-00-00-00-00-00", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableDateTimeSimple_Nullable() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableDateTimeSimple(null);
            Assert.AreEqual("01-00-00", encoder.GetBuffer().ToString());
        }

        [TestMethod]
        public void WriteIPAddress_IPv4() {
            var encoder = new ClassicEncoder();
            encoder.WriteIPAddress(IPAddress.Parse("1.2.3.4"));
            Assert.AreEqual("05-00-04-01-02-03-04", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableIPAddress_1() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableIPAddress(IPAddress.Parse("1.2.3.4"));
            Assert.AreEqual("05-00-04-01-02-03-04", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableIPAddress_IPv4() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableIPAddress(null);
            Assert.AreEqual("01-00-00", encoder.GetBuffer().ToString());
        }

        [TestMethod]
        public void WriteString_Test() {
            var encoder = new ClassicEncoder();
            encoder.WriteString("test");
            Assert.AreEqual("06-00-04-00-74-65-73-74", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableString_Test() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableString("test");
            Assert.AreEqual("07-00-01-04-00-74-65-73-74", encoder.GetBuffer().ToString());
        }
        [TestMethod]
        public void WriteNullableString_Null() {
            var encoder = new ClassicEncoder();
            encoder.WriteNullableString(null);
            Assert.AreEqual("01-00-00", encoder.GetBuffer().ToString());
        }

        [TestMethod]
        public void Write_Raw() {
            var encoder = new ClassicEncoder();
            encoder.Write(new byte[] { 1, 2, 3 });
            Assert.AreEqual("03-00-01-02-03", encoder.GetBuffer().ToString());
        }

        [TestMethod]
        public void EndToEnd() {
            using (var stream = new MemoryStream()) {
                using (var writer = new FeatherWriter(stream)) {
                    var payload = new ClassicEncoder();
                    payload.WriteUInt32(1);
                    payload.WriteBoolean(true);
                    payload.WriteString("test");
                    writer.Write(payload);

                    payload = new ClassicEncoder();
                    payload.WriteUInt32(2);
                    payload.WriteBoolean(false);
                    payload.WriteNullableString(null);
                    writer.Write(payload);
                }

                stream.Position = 0;

                using (var reader = new FeatherReader(stream)) {
                    var payload2 = reader.Read<ClassicDecoder>();
                    Assert.AreEqual((uint)1, payload2.ReadUInt32());
                    Assert.AreEqual(true, payload2.ReadBoolean());
                    Assert.AreEqual("test", payload2.ReadString());

                    payload2 = reader.Read<ClassicDecoder>();
                    Assert.AreEqual((uint)2, payload2.ReadUInt32());
                    Assert.AreEqual(false, payload2.ReadBoolean());
                    Assert.AreEqual(null, payload2.ReadNullableString());

                    Assert.AreEqual(null, reader.Read<ClassicDecoder>());
                }
            }

        }
    }
}
