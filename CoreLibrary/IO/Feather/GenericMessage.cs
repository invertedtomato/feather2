using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace InvertedTomato.IO.Feather {
    public sealed class GenericMessage : IMessage {
        public GenericMessage WriteUInt8(Byte value) {
            WriteByte(value);
            return this;
        }
        public GenericMessage WriteNullableUInt8(Byte? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteUInt8(value.Value);
            }

            return this;
        }

        public GenericMessage WriteSInt8(SByte value) {
            WriteByte((Byte)value);
            return this;
        }
        public GenericMessage WriteNullableSInt8(SByte? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteSInt8(value.Value);
            }

            return this;
        }

        public GenericMessage WriteUInt16(UInt16 value) {
            return Write(BitConverter.GetBytes(value));
        }
        public GenericMessage WriteNullableUInt16(UInt16? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteUInt16(value.Value);
            }

            return this;
        }

        public GenericMessage WriteSInt16(Int16 value) {
            return Write(BitConverter.GetBytes(value));
        }
        public GenericMessage WriteNullableSInt16(Int16? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteSInt16(value.Value);
            }

            return this;
        }

        public GenericMessage WriteUInt32(UInt32 value) {
            return Write(BitConverter.GetBytes(value));
        }
        public GenericMessage WriteNullableUInt32(UInt32? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteUInt32(value.Value);
            }

            return this;
        }

        public GenericMessage WriteSInt32(Int32 value) {
            return Write(BitConverter.GetBytes(value));
        }
        public GenericMessage WriteNullableSInt32(Int32? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteSInt32(value.Value);
            }

            return this;
        }

        public GenericMessage WriteUInt64(UInt64 value) {
            return Write(BitConverter.GetBytes(value));
        }
        public GenericMessage WriteNullableUInt64(UInt64? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteUInt64(value.Value);
            }

            return this;
        }

        public GenericMessage WriteSInt64(Int64 value) {
            return Write(BitConverter.GetBytes(value));
        }
        public GenericMessage WriteNullableSInt64(Int64? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteSInt64(value.Value);
            }

            return this;
        }

        public GenericMessage WriteFloat(Single value) {
            return Write(BitConverter.GetBytes(value));
        }
        public GenericMessage WriteNullableFloat(Single? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteFloat(value.Value);
            }

            return this;
        }

        public GenericMessage WriteDouble(Double value) {
            return Write(BitConverter.GetBytes(value));
        }
        public GenericMessage WriteNullableDouble(Double? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteDouble(value.Value);
            }

            return this;
        }

        public GenericMessage WriteBoolean(Boolean value) {
            return Write(new Byte[] { value ? (Byte)0x01 : (Byte)0x00 });
        }
        public GenericMessage WriteNullableBoolean(Boolean? value) {
            if (null == value) {
                WriteBoolean(false);
            } else {
                WriteBoolean(true);
                WriteBoolean(value.Value);
            }
            return this;
        }

        public GenericMessage WriteGuid(Guid value) {
            return Write(value.ToByteArray());
        }
        public GenericMessage WriteNullableGuid(Guid? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteGuid(value.Value);
            }

            return this;
        }

        public GenericMessage WriteTime(TimeSpan value) {
            return WriteSInt64(value.Ticks);
        }
        public GenericMessage WriteNullableTime(TimeSpan? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteTime(value.Value);
            }

            return this;
        }

        public GenericMessage WriteTimeSimple(TimeSpan value) {
            return WriteSInt64((Int64)value.TotalSeconds);
        }
        public GenericMessage WriteNullableTimeSimple(TimeSpan? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteTimeSimple(value.Value);
            }

            return this;
        }

        public GenericMessage WriteDateTime(DateTime value) {
            return WriteSInt64(value.Ticks);
        }
        public GenericMessage WriteNullableDateTime(DateTime? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteDateTime(value.Value);
            }

            return this;
        }

        public GenericMessage WriteDateTimeSimple(DateTime value) {
            return WriteSInt64(value.Ticks / TimeSpan.TicksPerMillisecond); // TODO
        }
        public GenericMessage WriteNullableDateTimeSimple(DateTime? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteDateTimeSimple(value.Value);
            }

            return this;
        }

        public GenericMessage WriteString(String value) {
            if (null == value) {
                throw new ArgumentNullException("value");
            }

            // Convert to byte array
            var raw = Encoding.UTF8.GetBytes(value);

            // Write length
            WriteUInt16((UInt16)raw.Length);

            // Write raw array
            Write(raw);

            return this;
        }
        public GenericMessage WriteNullableString(String value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteString(value);
            }

            return this;
        }

        public Byte ReadUInt8() {
            return SymbolBuffer.Dequeue();
        }
        public Byte? ReadNullableUInt8() {
            if(ReadBoolean()) {
                return ReadUInt8();
            } else {
                return null;
            }
        }

        public SByte ReadSInt8() {
            return (SByte)SymbolBuffer.Dequeue();
        }
        public SByte? ReadNullableSInt8() {
            if(ReadBoolean()) {
                return ReadSInt8();
            } else {
                return null;
            }
        }

        public UInt16 ReadUInt16() {
            var ret = SymbolBuffer.DequeueBuffer(2);
            return BitConverter.ToUInt16(ret.GetUnderlying(), ret.Start);
        }
        public UInt16? ReadNullableUInt16() {
            if(ReadBoolean()) {
                return ReadUInt16();
            } else {
                return null;
            }
        }

        public Int16 ReadSInt16() {
            var ret = SymbolBuffer.DequeueBuffer(2);
            return BitConverter.ToInt16(ret.GetUnderlying(), ret.Start);
        }
        public Int16? ReadNullableSInt16() {
            if(ReadBoolean()) {
                return ReadSInt16();
            } else {
                return null;
            }
        }

        public UInt32 ReadUInt32() {
            var ret = SymbolBuffer.DequeueBuffer(4);
            return BitConverter.ToUInt32(ret.GetUnderlying(), ret.Start);
        }
        public UInt32? ReadNullableUInt32() {
            if(ReadBoolean()) {
                return ReadUInt32();
            } else {
                return null;
            }
        }

        public Int32 ReadSInt32() {
            var ret = SymbolBuffer.DequeueBuffer(4);
            return BitConverter.ToInt32(ret.GetUnderlying(), ret.Start);
        }
        public Int32? ReadNullableSInt32() {
            if(ReadBoolean()) {
                return ReadSInt32();
            } else {
                return null;
            }
        }

        public UInt64 ReadUInt64() {
            var ret = SymbolBuffer.DequeueBuffer(8);
            return BitConverter.ToUInt64(ret.GetUnderlying(), ret.Start);
        }
        public UInt64? ReadNullableUInt64() {
            if(ReadBoolean()) {
                return ReadUInt64();
            } else {
                return null;
            }
        }

        public Int64 ReadSInt64() {
            var ret = SymbolBuffer.DequeueBuffer(8);
            return BitConverter.ToInt64(ret.GetUnderlying(), ret.Start);
        }
        public Int64? ReadNullableSInt64() {
            if(ReadBoolean()) {
                return ReadSInt64();
            } else {
                return null;
            }
        }

        public Single ReadFloat() {
            var ret = SymbolBuffer.DequeueBuffer(4);
            return BitConverter.ToSingle(ret.GetUnderlying(), ret.Start);
        }
        public Single? ReadNullableFloat() {
            if(ReadBoolean()) {
                return ReadFloat();
            } else {
                return null;
            }
        }

        public Double ReadDouble() {
            var ret = SymbolBuffer.DequeueBuffer(8);
            return BitConverter.ToDouble(ret.GetUnderlying(), ret.Start);
        }
        public Double? ReadNullableDouble() {
            if(ReadBoolean()) {
                return ReadDouble();
            } else {
                return null;
            }
        }

        public Boolean ReadBoolean() {
            return SymbolBuffer.Dequeue() > 0;
        }
        public Boolean? ReadNullableBoolean() {
            if(ReadBoolean()) {
                return ReadBoolean();
            } else {
                return null;
            }
        }

        public Guid ReadGuid() {
            var ret = SymbolBuffer.DequeueBuffer(16);
            return new Guid(ret.DequeueBuffer(16).ToArray());
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

            var raw = SymbolBuffer.DequeueBuffer(length).ToArray();
            return Encoding.UTF8.GetString(raw, 0, raw.Length);
        }
        public String ReadNullableString() {
            if(ReadBoolean()) {
                return ReadString();
            } else {
                return null;
            }
        }
    }
}
