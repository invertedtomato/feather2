using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;
using System.Text;

namespace InvertedTomato.IO.Feather {
    public sealed class MessageDecoder<TCodec> where TCodec : IIntegerCodec, new() {
        private Buffer<ulong> Symbols;

        public MessageDecoder() {
            Symbols = new Buffer<ulong>(0);
        }
        public MessageDecoder(Buffer<byte> payload) {
#if DEBUG
            if (null == payload) {
                throw new ArgumentNullException("payload");
            }
#endif
            
            // Decompress payload into symbols
            var codec = new TCodec();
            Symbols = new Buffer<ulong>(payload.Used);
            while (!codec.Decompress(payload, Symbols)) {
                Symbols = Symbols.Resize(Symbols.MaxCapacity * 2);
            }
        }


        public byte ReadUInt8() {
            return SymbolBuffer.Dequeue();
        }
        public byte? ReadNullableUInt8() {
            if (ReadBoolean()) {
                return ReadUInt8();
            } else {
                return null;
            }
        }

        public sbyte ReadSInt8() {
            return (sbyte)SymbolBuffer.Dequeue();
        }
        public sbyte? ReadNullableSInt8() {
            if (ReadBoolean()) {
                return ReadSInt8();
            } else {
                return null;
            }
        }

        public ushort ReadUInt16() {
            var ret = SymbolBuffer.DequeueBuffer(2);
            return BitConverter.ToUInt16(ret.GetUnderlying(), ret.Start);
        }
        public ushort? ReadNullableUInt16() {
            if (ReadBoolean()) {
                return ReadUInt16();
            } else {
                return null;
            }
        }

        public short ReadSInt16() {
            var ret = SymbolBuffer.DequeueBuffer(2);
            return BitConverter.ToInt16(ret.GetUnderlying(), ret.Start);
        }
        public short? ReadNullableSInt16() {
            if (ReadBoolean()) {
                return ReadSInt16();
            } else {
                return null;
            }
        }

        public uint ReadUInt32() {
            var ret = SymbolBuffer.DequeueBuffer(4);
            return BitConverter.ToUInt32(ret.GetUnderlying(), ret.Start);
        }
        public uint? ReadNullableUInt32() {
            if (ReadBoolean()) {
                return ReadUInt32();
            } else {
                return null;
            }
        }

        public int ReadSInt32() {
            var ret = SymbolBuffer.DequeueBuffer(4);
            return BitConverter.ToInt32(ret.GetUnderlying(), ret.Start);
        }
        public int? ReadNullableSInt32() {
            if (ReadBoolean()) {
                return ReadSInt32();
            } else {
                return null;
            }
        }

        public ulong ReadUInt64() {
            var ret = SymbolBuffer.DequeueBuffer(8);
            return BitConverter.ToUInt64(ret.GetUnderlying(), ret.Start);
        }
        public ulong? ReadNullableUInt64() {
            if (ReadBoolean()) {
                return ReadUInt64();
            } else {
                return null;
            }
        }

        public long ReadSInt64() {
            var ret = SymbolBuffer.DequeueBuffer(8);
            return BitConverter.ToInt64(ret.GetUnderlying(), ret.Start);
        }
        public long? ReadNullableSInt64() {
            if (ReadBoolean()) {
                return ReadSInt64();
            } else {
                return null;
            }
        }

        public float ReadFloat() {
            var ret = SymbolBuffer.DequeueBuffer(4);
            return BitConverter.ToSingle(ret.GetUnderlying(), ret.Start);
        }
        public float? ReadNullableFloat() {
            if (ReadBoolean()) {
                return ReadFloat();
            } else {
                return null;
            }
        }

        public double ReadDouble() {
            var ret = SymbolBuffer.DequeueBuffer(8);
            return BitConverter.ToDouble(ret.GetUnderlying(), ret.Start);
        }
        public double? ReadNullableDouble() {
            if (ReadBoolean()) {
                return ReadDouble();
            } else {
                return null;
            }
        }

        public bool ReadBoolean() {
            return SymbolBuffer.Dequeue() > 0;
        }
        public bool? ReadNullableBoolean() {
            if (ReadBoolean()) {
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
            if (ReadBoolean()) {
                return ReadGuid();
            } else {
                return null;
            }
        }

        public TimeSpan ReadTime() {
            return new TimeSpan(ReadSInt64());
        }
        public TimeSpan? ReadNullableTime() {
            if (ReadBoolean()) {
                return ReadTime();
            } else {
                return null;
            }
        }

        public TimeSpan ReadTimeSimple() {
            return new TimeSpan(ReadSInt64() * TimeSpan.TicksPerSecond);
        }
        public TimeSpan? ReadNullableTimeSimple() {
            if (ReadBoolean()) {
                return ReadTimeSimple();
            } else {
                return null;
            }
        }

        public DateTime ReadDateTime() {
            return new DateTime(ReadSInt64());
        }
        public DateTime? ReadNullableDateTime() {
            if (ReadBoolean()) {
                return ReadDateTime();
            } else {
                return null;
            }
        }

        public DateTime ReadDateTimeSimple() {
            return new DateTime(ReadSInt64() * TimeSpan.TicksPerMillisecond);
        }
        public DateTime? ReadNullableDateTimeSimple() {
            if (ReadBoolean()) {
                return ReadDateTimeSimple();
            } else {
                return null;
            }
        }

        public string ReadString() {
            var length = ReadUInt16();

            var raw = SymbolBuffer.DequeueBuffer(length).ToArray();
            return Encoding.UTF8.GetString(raw, 0, raw.Length);
        }
        public string ReadNullableString() {
            if (ReadBoolean()) {
                return ReadString();
            } else {
                return null;
            }
        }
        
        // TODO: byte array
    }
}
