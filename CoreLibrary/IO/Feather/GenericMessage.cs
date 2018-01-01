﻿using InvertedTomato.Compression.Integers;
using System;
using System.IO;
using System.Text;

namespace InvertedTomato.IO.Feather {
    public sealed class GenericMessage : MemoryStream, IMessage {
        private readonly VLQCodec VLQ = new VLQCodec();

        public GenericMessage WriteUnsignedInteger(UInt64 value) {
            VLQ.CompressUnsigned(this, value);
            return this;
        }
        public GenericMessage WriteNullableUnsignedInteger(UInt64? value) {
            if(null == value) {
                WriteUnsignedInteger(0);
            } else {
                WriteUnsignedInteger(1);
                WriteUnsignedInteger(value.Value);
            }

            return this;
        }

        public GenericMessage WriteSignedInteger(Int64 value) {
            VLQ.CompressSigned(this, value);
            return this;
        }
        public GenericMessage WriteNullableSignedInteger(Int64? value) {
            if(null == value) {
                WriteUnsignedInteger(0);
            } else {
                WriteUnsignedInteger(1);
                WriteSignedInteger(value.Value);
            }

            return this;
        }
        
        public GenericMessage WriteFloat(Single value) {
            return WriteByteArray(BitConverter.GetBytes(value));
        }
        public GenericMessage WriteNullableFloat(Single? value) {
            if(null == value) {
                WriteUnsignedInteger(0);
            } else {
                WriteUnsignedInteger(1);
                WriteFloat(value.Value);
            }

            return this;
        }

        public GenericMessage WriteDouble(Double value) {
            return WriteByteArray(BitConverter.GetBytes(value));
        }
        public GenericMessage WriteNullableDouble(Double? value) {
            if(null == value) {
                WriteUnsignedInteger(0);
            } else {
                WriteUnsignedInteger(1);
                WriteDouble(value.Value);
            }

            return this;
        }

        public GenericMessage WriteBoolean(Boolean value) {
            return WriteByteArray(new Byte[] { value ? (Byte)0x01 : (Byte)0x00 });
        }
        public GenericMessage WriteNullableBoolean(Boolean? value) {
            if(null == value) {
                WriteBoolean(false);
            } else {
                WriteBoolean(true);
                WriteBoolean(value.Value);
            }
            return this;
        }

        public GenericMessage WriteGuid(Guid value) {
            return WriteByteArray(value.ToByteArray());
        }
        public GenericMessage WriteNullableGuid(Guid? value) {
            if(null == value) {
                WriteUnsignedInteger(0);
            } else {
                WriteUnsignedInteger(1);
                WriteGuid(value.Value);
            }

            return this;
        }

        public GenericMessage WriteTime(TimeSpan value) {
            return WriteSignedInteger(value.Ticks);
        }
        public GenericMessage WriteNullableTime(TimeSpan? value) {
            if(null == value) {
                WriteUnsignedInteger(0);
            } else {
                WriteUnsignedInteger(1);
                WriteTime(value.Value);
            }

            return this;
        }

        public GenericMessage WriteTimeSimple(TimeSpan value) {
            return WriteSignedInteger((Int64)value.TotalSeconds);
        }
        public GenericMessage WriteNullableTimeSimple(TimeSpan? value) {
            if(null == value) {
                WriteUnsignedInteger(0);
            } else {
                WriteUnsignedInteger(1);
                WriteTimeSimple(value.Value);
            }

            return this;
        }

        public GenericMessage WriteDateTime(DateTime value) {
            return WriteSignedInteger(value.Ticks);
        }
        public GenericMessage WriteNullableDateTime(DateTime? value) {
            if(null == value) {
                WriteUnsignedInteger(0);
            } else {
                WriteUnsignedInteger(1);
                WriteDateTime(value.Value);
            }

            return this;
        }

        public GenericMessage WriteDateTimeSimple(DateTime value) {
            return WriteSignedInteger(value.Ticks / TimeSpan.TicksPerMillisecond); // TODO
        }
        public GenericMessage WriteNullableDateTimeSimple(DateTime? value) {
            if(null == value) {
                WriteUnsignedInteger(0);
            } else {
                WriteUnsignedInteger(1);
                WriteDateTimeSimple(value.Value);
            }

            return this;
        }

        public GenericMessage WriteString(String value) {
            if(null == value) {
                throw new ArgumentNullException(nameof(value));
            }

            // Convert to byte array
            var raw = Encoding.UTF8.GetBytes(value);

            // Write length
            WriteUnsignedInteger((UInt64)raw.Length);

            // Write raw array
            WriteByteArray(raw);

            return this;
        }
        public GenericMessage WriteNullableString(String value) {
            if(null == value) {
                WriteUnsignedInteger(0);
            } else {
                WriteUnsignedInteger(1);
                WriteString(value);
            }

            return this;
        }

        public GenericMessage WriteByteArray(Byte[] value) {
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

        public GenericMessage WriteByteArray(Byte[] value, Int32 offset, Int32 count) {
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
