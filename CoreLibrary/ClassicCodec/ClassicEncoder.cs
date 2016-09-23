using InvertedTomato.Buffers;
using System;
using ThreePlay.IO.Feather;

namespace InvertedTomato.IO.Feather.ClassicCodec {
    public class ClassicEncoder : IEncoder {
        public ClassicEncoder WriteUInt8(byte value) { throw new NotImplementedException(); }
        public ClassicEncoder WriteNullableUInt8(byte? value) { throw new NotImplementedException(); }

        public ClassicEncoder WriteSInt8(sbyte value) { throw new NotImplementedException(); }
        public ClassicEncoder WriteNullableSInt8(sbyte? value) { throw new NotImplementedException(); }

        public ClassicEncoder WriteUInt16(ushort value) { throw new NotImplementedException(); }
        public ClassicEncoder WriteNullableUInt16(ushort? value) { throw new NotImplementedException(); }

        public ClassicEncoder WriteSInt16(short value) { throw new NotImplementedException(); }
        public ClassicEncoder WriteNullableSInt16(short? value) { throw new NotImplementedException(); }

        public ClassicEncoder WriteUInt32(uint value) { throw new NotImplementedException(); }
        public ClassicEncoder WriteNullableUInt32(uint? value) { throw new NotImplementedException(); }

        public ClassicEncoder WriteSInt32(int value) { throw new NotImplementedException(); }
        public ClassicEncoder WriteNullableSInt32(int? value) { throw new NotImplementedException(); }

        public ClassicEncoder WriteUInt64(ulong value) { throw new NotImplementedException(); }
        public ClassicEncoder WriteNullableUInt64(ulong? value) { throw new NotImplementedException(); }

        public ClassicEncoder WriteSInt64(long value) { throw new NotImplementedException(); }
        public ClassicEncoder WriteNullableSInt64(long? value) { throw new NotImplementedException(); }

        public ClassicEncoder WriteFloat(float value) { throw new NotImplementedException(); }
        public ClassicEncoder WriteNullableFloat(float? value) { throw new NotImplementedException(); }

        public ClassicEncoder WriteDouble(double value) { throw new NotImplementedException(); }
        public ClassicEncoder WriteNullableDouble(double? value) { throw new NotImplementedException(); }

        public ClassicEncoder WriteBoolean(bool value) { throw new NotImplementedException(); }
        public ClassicEncoder WriteNullableBoolean(bool? value) { throw new NotImplementedException(); }

        public ClassicEncoder WriteGuid(Guid value) { throw new NotImplementedException(); }
        public ClassicEncoder WriteNullableGuid(Guid? value) { throw new NotImplementedException(); }

        public ClassicEncoder WriteTime(TimeSpan value) { throw new NotImplementedException(); }
        public ClassicEncoder WriteNullableTime(TimeSpan? value) { throw new NotImplementedException(); }

        public ClassicEncoder WriteDateTime(DateTime value) { throw new NotImplementedException(); }
        public ClassicEncoder WriteNullableDateTime(DateTime? value) { throw new NotImplementedException(); }

        public ReadOnlyBuffer<byte> ToBuffer() {
            throw new NotImplementedException();
        }
    }
}
