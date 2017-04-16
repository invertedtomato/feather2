using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;
using System.Net;
using System.Text;

namespace InvertedTomato.IO.Feather {
    public sealed class MessageEncoder<TCodec> where TCodec : IIntegerCodec, new() {
        private const int SymbolBufferDefaultSize = 8;
        private const int SymbolBufferGrowthRate = 2;

        private Buffer<ulong> Symbols;
        private Buffer<byte> Payload = null;

        public bool IsReadOnly { get { return null != Payload; } }

        public MessageEncoder() {
            Symbols = new Buffer<ulong>(SymbolBufferDefaultSize);
        }
        public MessageEncoder(int initialCapacity) {
#if DEBUG
            if (initialCapacity < 1) {
                throw new ArgumentOutOfRangeException("Must be at least 1 byte.", "initialCapacity");
            }
#endif

            Symbols = new Buffer<ulong>(initialCapacity);
        }

        public MessageEncoder<TCodec> WriteUInt8(byte value) {
            return Write(new byte[] { value });
        }
        public MessageEncoder<TCodec> WriteNullableUInt8(byte? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteUInt8(value.Value);
            }

            return this;
        }

        public MessageEncoder<TCodec> WriteSInt8(sbyte value) {
            return Write(new byte[] { (byte)value });
        }
        public MessageEncoder<TCodec> WriteNullableSInt8(sbyte? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteSInt8(value.Value);
            }

            return this;
        }

        public MessageEncoder<TCodec> WriteUInt16(ushort value) {
            return Write(BitConverter.GetBytes(value));
        }
        public MessageEncoder<TCodec> WriteNullableUInt16(ushort? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteUInt16(value.Value);
            }

            return this;
        }

        public MessageEncoder<TCodec> WriteSInt16(short value) {
            return Write(BitConverter.GetBytes(value));
        }
        public MessageEncoder<TCodec> WriteNullableSInt16(short? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteSInt16(value.Value);
            }

            return this;
        }

        public MessageEncoder<TCodec> WriteUInt32(uint value) {
            return Write(BitConverter.GetBytes(value));
        }
        public MessageEncoder<TCodec> WriteNullableUInt32(uint? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteUInt32(value.Value);
            }

            return this;
        }

        public MessageEncoder<TCodec> WriteSInt32(int value) {
            return Write(BitConverter.GetBytes(value));
        }
        public MessageEncoder<TCodec> WriteNullableSInt32(int? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteSInt32(value.Value);
            }

            return this;
        }

        public MessageEncoder<TCodec> WriteUInt64(ulong value) {
            return Write(BitConverter.GetBytes(value));
        }
        public MessageEncoder<TCodec> WriteNullableUInt64(ulong? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteUInt64(value.Value);
            }

            return this;
        }

        public MessageEncoder<TCodec> WriteSInt64(long value) {
            return Write(BitConverter.GetBytes(value));
        }
        public MessageEncoder<TCodec> WriteNullableSInt64(long? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteSInt64(value.Value);
            }

            return this;
        }

        public MessageEncoder<TCodec> WriteFloat(float value) {
            return Write(BitConverter.GetBytes(value));
        }
        public MessageEncoder<TCodec> WriteNullableFloat(float? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteFloat(value.Value);
            }

            return this;
        }

        public MessageEncoder<TCodec> WriteDouble(double value) {
            return Write(BitConverter.GetBytes(value));
        }
        public MessageEncoder<TCodec> WriteNullableDouble(double? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteDouble(value.Value);
            }

            return this;
        }

        public MessageEncoder<TCodec> WriteBoolean(bool value) {
            return Write(new byte[] { value ? (byte)0x01 : (byte)0x00 });
        }
        public MessageEncoder<TCodec> WriteNullableBoolean(bool? value) {
            if (null == value) {
                WriteBoolean(false);
            } else {
                WriteBoolean(true);
                WriteBoolean(value.Value);
            }
            return this;
        }

        public MessageEncoder<TCodec> WriteGuid(Guid value) {
            return Write(value.ToByteArray());
        }
        public MessageEncoder<TCodec> WriteNullableGuid(Guid? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteGuid(value.Value);
            }

            return this;
        }

        public MessageEncoder<TCodec> WriteTime(TimeSpan value) {
            return WriteSInt64(value.Ticks);
        }
        public MessageEncoder<TCodec> WriteNullableTime(TimeSpan? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteTime(value.Value);
            }

            return this;
        }

        public MessageEncoder<TCodec> WriteTimeSimple(TimeSpan value) {
            return WriteSInt64((long)value.TotalSeconds);
        }
        public MessageEncoder<TCodec> WriteNullableTimeSimple(TimeSpan? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteTimeSimple(value.Value);
            }

            return this;
        }

        public MessageEncoder<TCodec> WriteDateTime(DateTime value) {
            return WriteSInt64(value.Ticks);
        }
        public MessageEncoder<TCodec> WriteNullableDateTime(DateTime? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteDateTime(value.Value);
            }

            return this;
        }

        public MessageEncoder<TCodec> WriteDateTimeSimple(DateTime value) {
            return WriteSInt64(value.Ticks / TimeSpan.TicksPerMillisecond); // TODO
        }
        public MessageEncoder<TCodec> WriteNullableDateTimeSimple(DateTime? value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteDateTimeSimple(value.Value);
            }

            return this;
        }

        public MessageEncoder<TCodec> WriteString(string value) {
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
        public MessageEncoder<TCodec> WriteNullableString(string value) {
            if (null == value) {
                WriteUInt8(0);
            } else {
                WriteUInt8(1);
                WriteString(value);
            }

            return this;
        }

        // TODO: byte array

        private void Write(ulong value) {
#if DEBUG
            if (IsReadOnly) {
                throw new InvalidOperationException("Message has been submitted and encoder is now ready-only.");
            }
#endif

            // If symbol buffer is full...
            if (Symbols.IsFull) {
                // Grow it
                Symbols = Symbols.Resize(Symbols.MaxCapacity * SymbolBufferGrowthRate);
            }

            // Add to queue
            Symbols.Enqueue(value);
        }

        public ReadOnlyBuffer<byte> GetPayload() {
            // If payload hasn't been computed
            if (null == Payload) {

                // Compress
                var codec = new TCodec();
                Payload = new Buffer<byte>(Symbols.Used * 2);
                while (!codec.Compress(Symbols, Payload)) {
                    Payload = Payload.Resize(Payload.MaxCapacity * 2);
                }
            }

            return Payload;
        }
    }
}
