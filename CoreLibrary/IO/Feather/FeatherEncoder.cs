using InvertedTomato.IO.Buffers;
using System;
using System.Net;
using System.Text;

namespace InvertedTomato.IO.Feather {
    public sealed class FeatherEncoder {
        private Buffer<byte> SymbolBuffer = new Buffer<byte>(8);

        public FeatherEncoder() {
            // Reserve space for length
            SymbolBuffer.Enqueue(0);
            SymbolBuffer.Enqueue(0);
        }

        public FeatherEncoder WriteUInt8(byte value) {
            return Write(new byte[] { value });
        }
        public FeatherEncoder WriteNullableUInt8(byte? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteUInt8(value.Value);
            }

            return this;
        }

        public FeatherEncoder WriteSInt8(sbyte value) {
            return Write(new byte[] { (byte)value });
        }
        public FeatherEncoder WriteNullableSInt8(sbyte? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteSInt8(value.Value);
            }

            return this;
        }

        public FeatherEncoder WriteUInt16(ushort value) {
            return Write(BitConverter.GetBytes(value));
        }
        public FeatherEncoder WriteNullableUInt16(ushort? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteUInt16(value.Value);
            }

            return this;
        }

        public FeatherEncoder WriteSInt16(short value) {
            return Write(BitConverter.GetBytes(value));
        }
        public FeatherEncoder WriteNullableSInt16(short? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteSInt16(value.Value);
            }

            return this;
        }

        public FeatherEncoder WriteUInt32(uint value) {
            return Write(BitConverter.GetBytes(value));
        }
        public FeatherEncoder WriteNullableUInt32(uint? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteUInt32(value.Value);
            }

            return this;
        }

        public FeatherEncoder WriteSInt32(int value) {
            return Write(BitConverter.GetBytes(value));
        }
        public FeatherEncoder WriteNullableSInt32(int? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteSInt32(value.Value);
            }

            return this;
        }

        public FeatherEncoder WriteUInt64(ulong value) {
            return Write(BitConverter.GetBytes(value));
        }
        public FeatherEncoder WriteNullableUInt64(ulong? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteUInt64(value.Value);
            }

            return this;
        }

        public FeatherEncoder WriteSInt64(long value) {
            return Write(BitConverter.GetBytes(value));
        }
        public FeatherEncoder WriteNullableSInt64(long? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteSInt64(value.Value);
            }

            return this;
        }

        public FeatherEncoder WriteFloat(float value) {
            return Write(BitConverter.GetBytes(value));
        }
        public FeatherEncoder WriteNullableFloat(float? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteFloat(value.Value);
            }

            return this;
        }

        public FeatherEncoder WriteDouble(double value) {
            return Write(BitConverter.GetBytes(value));
        }
        public FeatherEncoder WriteNullableDouble(double? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteDouble(value.Value);
            }

            return this;
        }

        public FeatherEncoder WriteBoolean(bool value) {
            return Write(new byte[] { value ? (byte)0x01 : (byte)0x00 });
        }
        public FeatherEncoder WriteNullableBoolean(bool? value) {
            if (null == value) {
                WriteBoolean(false);
            } else {
                WriteBoolean(true);
                WriteBoolean(value.Value);
            }
            return this;
        }

        public FeatherEncoder WriteGuid(Guid value) {
            return Write(value.ToByteArray());
        }
        public FeatherEncoder WriteNullableGuid(Guid? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteGuid(value.Value);
            }

            return this;
        }

        public FeatherEncoder WriteTime(TimeSpan value) {
            return WriteSInt64(value.Ticks);
        }
        public FeatherEncoder WriteNullableTime(TimeSpan? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteTime(value.Value);
            }

            return this;
        }

        public FeatherEncoder WriteTimeSimple(TimeSpan value) {
            return WriteSInt64((long)value.TotalSeconds);
        }
        public FeatherEncoder WriteNullableTimeSimple(TimeSpan? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteTimeSimple(value.Value);
            }

            return this;
        }

        public FeatherEncoder WriteDateTime(DateTime value) {
            return WriteSInt64(value.Ticks);
        }
        public FeatherEncoder WriteNullableDateTime(DateTime? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteDateTime(value.Value);
            }

            return this;
        }

        public FeatherEncoder WriteDateTimeSimple(DateTime value) {
            return WriteSInt64(value.Ticks / TimeSpan.TicksPerMillisecond); // TODO
        }
        public FeatherEncoder WriteNullableDateTimeSimple(DateTime? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteDateTimeSimple(value.Value);
            }

            return this;
        }
        
        public FeatherEncoder WriteString(string value) {
            if (null == value) {
                throw new ArgumentNullException("value");
            }

            // Convert to byte array
            var raw = Encoding.UTF8.GetBytes(value);

            // Write length
            WriteUInt16((ushort)raw.Length);

            // Write raw array
            Write(raw);

            return this;
        }
        public FeatherEncoder WriteNullableString(string value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteString(value);
            }

            return this;
        }

        public FeatherEncoder Write(byte[] value) {
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
