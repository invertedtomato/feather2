using InvertedTomato.Buffers;
using System;
using ThreePlay.IO.Feather;

namespace InvertedTomato.IO.Feather.ClassicCodec {
    public class ClassicDecoder : IDecoder {
        public int MaxHeaderLength { get { return 2; } }

        public byte ReadUInt8() { throw new NotImplementedException(); }
        public byte? ReadNullableUInt8() { throw new NotImplementedException(); }

        public sbyte ReadSInt8() { throw new NotImplementedException(); }
        public sbyte? ReadNullableSInt8() { throw new NotImplementedException(); }

        public ushort ReadUInt16() { throw new NotImplementedException(); }
        public ushort? ReadNullableUInt16() { throw new NotImplementedException(); }

        public short ReadSInt16() { throw new NotImplementedException(); }
        public short? ReadNullableSInt16() { throw new NotImplementedException(); }

        public uint ReadUInt32() { throw new NotImplementedException(); }
        public uint? ReadNullableUInt32() { throw new NotImplementedException(); }

        public int ReadSInt32() { throw new NotImplementedException(); }
        public int? ReadNullableSInt32() { throw new NotImplementedException(); }

        public ulong ReadUInt64() { throw new NotImplementedException(); }
        public ulong? ReadNullableUInt64() { throw new NotImplementedException(); }

        public long ReadSInt64() { throw new NotImplementedException(); }
        public long? ReadNullableSInt64() { throw new NotImplementedException(); }

        public float ReadFloat() { throw new NotImplementedException(); }
        public float? ReadNullableFloat() { throw new NotImplementedException(); }

        public double ReadDouble() { throw new NotImplementedException(); }
        public double? ReadNullableDouble() { throw new NotImplementedException(); }

        public bool ReadBoolean() { throw new NotImplementedException(); }
        public bool? ReadNullableBoolean() { throw new NotImplementedException(); }

        public Guid ReadGuid() { throw new NotImplementedException(); }
        public Guid? ReadNullableGuid() { throw new NotImplementedException(); }

        public TimeSpan ReadTime() { throw new NotImplementedException(); }
        public TimeSpan? ReadNullableTime() { throw new NotImplementedException(); }

        public DateTime ReadDateTime() { throw new NotImplementedException(); }
        public DateTime? ReadNullableDateTime() { throw new NotImplementedException(); }
        
        public int GetPayloadLength(ReadOnlyBuffer<byte> buffer) {
            throw new NotImplementedException();
        }

        public bool LoadBuffer(Buffer<byte> buffer) {
            throw new NotImplementedException();
        }
    }
}
