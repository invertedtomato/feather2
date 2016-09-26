using InvertedTomato.Buffers;
using System;
using ThreePlay.IO.Feather;

namespace InvertedTomato.IO.Feather.ClassicCodec {
    public class ClassicEncoder : IEncoder {
        private Buffer<byte> SymbolBuffer = new Buffer<byte>(8);


        public ClassicEncoder() {
            // Reserve space for length
            SymbolBuffer.Enqueue(0);
            SymbolBuffer.Enqueue(0);
        }

        // TODO: Unit tests!

        public ClassicEncoder WriteUInt8(byte value) {
            return Write(new byte[] { value });
        }
        public ClassicEncoder WriteNullableUInt8(byte? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteUInt8(value.Value);
            }

            return this;
        }

        public ClassicEncoder WriteSInt8(sbyte value) {
            return Write(new byte[] { (byte)value });
        }
        public ClassicEncoder WriteNullableSInt8(sbyte? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteSInt8(value.Value);
            }

            return this;
        }

        public ClassicEncoder WriteUInt16(ushort value) {
            return Write(BitConverter.GetBytes(value));
        }
        public ClassicEncoder WriteNullableUInt16(ushort? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteUInt16(value.Value);
            }

            return this;
        }

        public ClassicEncoder WriteSInt16(short value) {
            return Write(BitConverter.GetBytes(value));
        }
        public ClassicEncoder WriteNullableSInt16(short? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteSInt16(value.Value);
            }

            return this;
        }

        public ClassicEncoder WriteUInt32(uint value) {
            return Write(BitConverter.GetBytes(value));
        }
        public ClassicEncoder WriteNullableUInt32(uint? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteUInt32(value.Value);
            }

            return this;
        }

        public ClassicEncoder WriteSInt32(int value) {
            return Write(BitConverter.GetBytes(value));
        }
        public ClassicEncoder WriteNullableSInt32(int? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteSInt32(value.Value);
            }

            return this;
        }

        public ClassicEncoder WriteUInt64(ulong value) {
            return Write(BitConverter.GetBytes(value));
        }
        public ClassicEncoder WriteNullableUInt64(ulong? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteUInt64(value.Value);
            }

            return this;
        }

        public ClassicEncoder WriteSInt64(long value) {
            return Write(BitConverter.GetBytes(value));
        }
        public ClassicEncoder WriteNullableSInt64(long? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteSInt64(value.Value);
            }

            return this;
        }

        public ClassicEncoder WriteFloat(float value) {
            return Write(BitConverter.GetBytes(value));
        }
        public ClassicEncoder WriteNullableFloat(float? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteFloat(value.Value);
            }

            return this;
        }

        public ClassicEncoder WriteDouble(double value) {
            return Write(BitConverter.GetBytes(value));
        }
        public ClassicEncoder WriteNullableDouble(double? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteDouble(value.Value);
            }

            return this;
        }

        public ClassicEncoder WriteBoolean(bool value) {
            return Write(BitConverter.GetBytes(value));
        }
        public ClassicEncoder WriteNullableBoolean(bool? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteBoolean(value.Value);
            }

            return this;
        }

        public ClassicEncoder WriteGuid(Guid value) {
            return Write(value.ToByteArray());
        }
        public ClassicEncoder WriteNullableGuid(Guid? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteNullableGuid(value.Value);
            }

            return this;
        }

        public ClassicEncoder WriteTime(TimeSpan value) {
            return WriteSInt64(value.Ticks);
        }
        public ClassicEncoder WriteNullableTime(TimeSpan? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteTime(value.Value);
            }

            return this;
        }

        public ClassicEncoder WriteTimeInSeconds(TimeSpan value) {
            return WriteDouble(value.TotalSeconds);
        }
        public ClassicEncoder WriteNullableTimeInSeconds(TimeSpan? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteTimeInSeconds(value.Value);
            }

            return this;
        }

        public ClassicEncoder WriteDateTime(DateTime value) {
            return WriteSInt64(value.Ticks);
        }
        public ClassicEncoder WriteNullableDateTime(DateTime? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteDateTime(value.Value);
            }

            return this;
        }

        public ClassicEncoder WriteDateTimeInSeconds(DateTime value) {
            return WriteUInt64(value.ToUnixTimeAsUInt64());
        }
        public ClassicEncoder WriteNullableDateTimeInSeconds(DateTime? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteDateTimeInSeconds(value.Value);
            }

            return this;
        }

        public ClassicEncoder Write(byte[] value) {
            if (null == value) {
                throw new ArgumentNullException("value");
            }

            // If there isn't enough space...
            if (SymbolBuffer.Available < value.Length) {
                // Calculate new size
                var size = Math.Max(SymbolBuffer.MaxCapacity * 2, SymbolBuffer.Used + value.Length); // Either double the size, or make it big enough - whichever is greater

                // Resize
                SymbolBuffer = SymbolBuffer.Resize(size);
            }

            // Enqueue
            SymbolBuffer.EnqueueArray(value);

            return this;
        }

        public ReadOnlyBuffer<byte> GetBuffer() {
            // Update length header
            var lengthHeader = BitConverter.GetBytes((ushort)(SymbolBuffer.Used - 2));
            SymbolBuffer.Replace(0, lengthHeader[0]);
            SymbolBuffer.Replace(1, lengthHeader[1]);

            // Return buffer
            return SymbolBuffer;
        }
    }
}
