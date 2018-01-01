using System;
using System.IO;
using System.Text;

namespace InvertedTomato.IO.Feather {
    public sealed class ClassicMessage : MemoryStream, IMessage {
        public ClassicMessage WriteUInt8(Byte value) {
            WriteByte(value);
            return this;
        }
        public ClassicMessage WriteNullableUInt8(Byte? value) {
            if(null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteUInt8(value.Value);
            }

            return this;
        }

        public ClassicMessage WriteSInt8(SByte value) {
            WriteByte((Byte)value);
            return this;
        }
        public ClassicMessage WriteNullableSInt8(SByte? value) {
            if(null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteSInt8(value.Value);
            }

            return this;
        }

        public ClassicMessage WriteUInt16(UInt16 value) {
            return WriteByteArray(BitConverter.GetBytes(value));
        }
        public ClassicMessage WriteNullableUInt16(UInt16? value) {
            if(null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteUInt16(value.Value);
            }

            return this;
        }

        public ClassicMessage WriteSInt16(Int16 value) {
            return WriteByteArray(BitConverter.GetBytes(value));
        }
        public ClassicMessage WriteNullableSInt16(Int16? value) {
            if(null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteSInt16(value.Value);
            }

            return this;
        }

        public ClassicMessage WriteUInt32(UInt32 value) {
            return WriteByteArray(BitConverter.GetBytes(value));
        }
        public ClassicMessage WriteNullableUInt32(UInt32? value) {
            if(null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteUInt32(value.Value);
            }

            return this;
        }

        public ClassicMessage WriteSInt32(Int32 value) {
            return WriteByteArray(BitConverter.GetBytes(value));
        }
        public ClassicMessage WriteNullableSInt32(Int32? value) {
            if(null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteSInt32(value.Value);
            }

            return this;
        }

        public ClassicMessage WriteUInt64(UInt64 value) {
            return WriteByteArray(BitConverter.GetBytes(value));
        }
        public ClassicMessage WriteNullableUInt64(UInt64? value) {
            if(null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteUInt64(value.Value);
            }

            return this;
        }

        public ClassicMessage WriteSInt64(Int64 value) {
            return WriteByteArray(BitConverter.GetBytes(value));
        }
        public ClassicMessage WriteNullableSInt64(Int64? value) {
            if(null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteSInt64(value.Value);
            }

            return this;
        }

        public ClassicMessage WriteFloat(Single value) {
            return WriteByteArray(BitConverter.GetBytes(value));
        }
        public ClassicMessage WriteNullableFloat(Single? value) {
            if(null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteFloat(value.Value);
            }

            return this;
        }

        public ClassicMessage WriteDouble(Double value) {
            return WriteByteArray(BitConverter.GetBytes(value));
        }
        public ClassicMessage WriteNullableDouble(Double? value) {
            if(null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteDouble(value.Value);
            }

            return this;
        }

        public ClassicMessage WriteBoolean(Boolean value) {
            return WriteByteArray(new Byte[] { value ? (Byte)0x01 : (Byte)0x00 });
        }
        public ClassicMessage WriteNullableBoolean(Boolean? value) {
            if(null == value) {
                WriteBoolean(false);
            } else {
                WriteBoolean(true);
                WriteBoolean(value.Value);
            }
            return this;
        }

        public ClassicMessage WriteGuid(Guid value) {
            return WriteByteArray(value.ToByteArray());
        }
        public ClassicMessage WriteNullableGuid(Guid? value) {
            if(null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteGuid(value.Value);
            }

            return this;
        }

        public ClassicMessage WriteTime(TimeSpan value) {
            return WriteSInt64(value.Ticks);
        }
        public ClassicMessage WriteNullableTime(TimeSpan? value) {
            if(null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteTime(value.Value);
            }

            return this;
        }

        public ClassicMessage WriteTimeSimple(TimeSpan value) {
            return WriteSInt64((Int64)value.TotalSeconds);
        }
        public ClassicMessage WriteNullableTimeSimple(TimeSpan? value) {
            if(null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteTimeSimple(value.Value);
            }

            return this;
        }

        public ClassicMessage WriteDateTime(DateTime value) {
            return WriteSInt64(value.Ticks);
        }
        public ClassicMessage WriteNullableDateTime(DateTime? value) {
            if(null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteDateTime(value.Value);
            }

            return this;
        }

        public ClassicMessage WriteDateTimeSimple(DateTime value) {
            return WriteSInt64(value.Ticks / TimeSpan.TicksPerMillisecond); // TODO
        }
        public ClassicMessage WriteNullableDateTimeSimple(DateTime? value) {
            if(null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteDateTimeSimple(value.Value);
            }

            return this;
        }

        public ClassicMessage WriteString(String value) {
            if(null == value) {
                throw new ArgumentNullException(nameof(value));
            }

            // Convert to byte array
            var raw = Encoding.UTF8.GetBytes(value);

            // Write length
            WriteUInt16((UInt16)raw.Length);

            // Write raw array
            WriteByteArray(raw);

            return this;
        }
        public ClassicMessage WriteNullableString(String value) {
            if(null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteString(value);
            }

            return this;
        }

        public ClassicMessage WriteByteArray(Byte[] value) {
            if(null == value) {
                throw new ArgumentNullException(nameof(value));
            }

            Write(value, 0, value.Length);

            return this;
        }
        public Byte[] ReadByteArray(Int32 count) {
            var buffer = new Byte[count];
            if(Read(buffer, 0, buffer.Length) != count) {
                throw new OverflowException();
            }

            return buffer;
        }

        public ClassicMessage WriteByteArray(Byte[] value, Int32 offset, Int32 count) {
            if(null == value) {
                throw new ArgumentNullException(nameof(value));
            }

            Write(value, offset, count);

            return this;
        }
        public Byte[] ReadByteArray(Int32 offset, Int32 count) {
            var buffer = new Byte[count];
            if(Read(buffer, offset, buffer.Length) != count) {
                throw new OverflowException();
            }

            return buffer;
        }




        public Byte ReadUInt8() {
            return ReadByteArray(1)[0];
        }
        public Byte? ReadNullableUInt8() {
            if(ReadBoolean()) {
                return ReadUInt8();
            } else {
                return null;
            }
        }

        public SByte ReadSInt8() {
            return (SByte)ReadByteArray(1)[0];
        }
        public SByte? ReadNullableSInt8() {
            if(ReadBoolean()) {
                return ReadSInt8();
            } else {
                return null;
            }
        }

        public UInt16 ReadUInt16() {
            return BitConverter.ToUInt16(ReadByteArray(2), 0);
        }
        public UInt16? ReadNullableUInt16() {
            if(ReadBoolean()) {
                return ReadUInt16();
            } else {
                return null;
            }
        }

        public Int16 ReadSInt16() {
            return BitConverter.ToInt16(ReadByteArray(2), 0);
        }
        public Int16? ReadNullableSInt16() {
            if(ReadBoolean()) {
                return ReadSInt16();
            } else {
                return null;
            }
        }

        public UInt32 ReadUInt32() {
            return BitConverter.ToUInt32(ReadByteArray(4), 0);
        }
        public UInt32? ReadNullableUInt32() {
            if(ReadBoolean()) {
                return ReadUInt32();
            } else {
                return null;
            }
        }

        public Int32 ReadSInt32() {
            return BitConverter.ToInt32(ReadByteArray(4), 0);
        }
        public Int32? ReadNullableSInt32() {
            if(ReadBoolean()) {
                return ReadSInt32();
            } else {
                return null;
            }
        }

        public UInt64 ReadUInt64() {
            return BitConverter.ToUInt64(ReadByteArray(8), 0);
        }
        public UInt64? ReadNullableUInt64() {
            if(ReadBoolean()) {
                return ReadUInt64();
            } else {
                return null;
            }
        }

        public Int64 ReadSInt64() {
            return BitConverter.ToInt64(ReadByteArray(8), 0);
        }
        public Int64? ReadNullableSInt64() {
            if(ReadBoolean()) {
                return ReadSInt64();
            } else {
                return null;
            }
        }

        public Single ReadFloat() {
            return BitConverter.ToSingle(ReadByteArray(4), 0);
        }
        public Single? ReadNullableFloat() {
            if(ReadBoolean()) {
                return ReadFloat();
            } else {
                return null;
            }
        }

        public Double ReadDouble() {
            return BitConverter.ToDouble(ReadByteArray(8), 0);
        }
        public Double? ReadNullableDouble() {
            if(ReadBoolean()) {
                return ReadDouble();
            } else {
                return null;
            }
        }

        public Boolean ReadBoolean() {
            return ReadUInt8() > 0;
        }
        public Boolean? ReadNullableBoolean() {
            if(ReadBoolean()) {
                return ReadBoolean();
            } else {
                return null;
            }
        }

        public Guid ReadGuid() {
            return new Guid(ReadByteArray(16));
        }
        public Guid? ReadNullableGuid() {
            if(ReadBoolean()) {
                return ReadGuid();
            } else {
                return null;
            }
        }

        public TimeSpan ReadTime() {
            return new TimeSpan(ReadSInt64());
        }
        public TimeSpan? ReadNullableTime() {
            if(ReadBoolean()) {
                return ReadTime();
            } else {
                return null;
            }
        }

        public TimeSpan ReadTimeSimple() {
            return new TimeSpan(ReadSInt64() * TimeSpan.TicksPerSecond);
        }
        public TimeSpan? ReadNullableTimeSimple() {
            if(ReadBoolean()) {
                return ReadTimeSimple();
            } else {
                return null;
            }
        }

        public DateTime ReadDateTime() {
            return new DateTime(ReadSInt64());
        }
        public DateTime? ReadNullableDateTime() {
            if(ReadBoolean()) {
                return ReadDateTime();
            } else {
                return null;
            }
        }

        public DateTime ReadDateTimeSimple() {
            return new DateTime(ReadSInt64() * TimeSpan.TicksPerMillisecond);
        }
        public DateTime? ReadNullableDateTimeSimple() {
            if(ReadBoolean()) {
                return ReadDateTimeSimple();
            } else {
                return null;
            }
        }

        public String ReadString() {
            var length = ReadUInt16();

            var raw = ReadByteArray(length);
            return Encoding.UTF8.GetString(raw, 0, raw.Length);
        }
        public String ReadNullableString() {
            if(ReadBoolean()) {
                return ReadString();
            } else {
                return null;
            }
        }


        public Byte[] ToByteArray() {
            return base.ToArray();
        }

        public void FromByteArray(Byte[] payload) {
            base.Write(payload, 0, payload.Length);
            base.Seek(0, SeekOrigin.Begin);
        }
    }
}
