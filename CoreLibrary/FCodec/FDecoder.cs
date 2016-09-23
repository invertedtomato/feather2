using System;
using System.IO;
using System.Net;
using ThreePlay.IO.Zero.Buffers;

namespace ThreePlay.IO.Zero.FCodec {
    public class FDecoder : IDecoder {
        public Buffer<ulong> Symbols;

        public FDecoder() { }
        public FDecoder(Buffer<ulong> symbols) {
            if (null == symbols) {
                throw new ArgumentNullException("symbols");
            }

            Symbols = symbols;
        }
        public bool LoadFromBuffer(Buffer<byte> buffer) {
#if DEBUG
            if (null == buffer) {
                throw new ArgumentNullException("buffer");
            }
            if (buffer.SubOffset != 0 && buffer.SubOffset != 8) {
                throw new ArgumentException("buffer", "SubPosition must be 0 or 8, not " + buffer.SubOffset + ".");
            }
#endif

            // Note initial positions
            var bytePosition = buffer.Offset;

            // Read length
            var messageLength = (int)F.DecodeNext(buffer);

            // Abort if insufficient bytes are available
            if (buffer.Count < messageLength) {
                // Restore byte position
                buffer.MoveAbsolute(bytePosition);

                return false;
            }

            // Allocate room for symbol set
            Symbols = new Buffer<ulong>((messageLength + 1) * 4); // Assumes max 4 symbols per byte

            // Read symbols
            var finalOffset = buffer.Offset + messageLength;
            while (buffer.Offset <= finalOffset && // Last byte
                buffer.SubOffset < 8) { // Last bit of last byte
                var symbol = F.DecodeNext(buffer);
                Symbols.Write(symbol);
            }

            return true;
        }

        public ulong? ReadUnsignedInteger() { return ReadUnsignedInteger(new FEncodingOptions()); }
        public ulong? ReadUnsignedInteger(FEncodingOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Isolate symbol;
            var symbol = Read();

            // Handle null support
            if (options.SupportNulls) {
                if (0 == symbol) {
                    return null;
                } else {
                    symbol--;
                }
            }

            // Handle arithmetic minimization
            return options.UnsignedIntegerMinimum + symbol * options.IntegerAccuracy;
        }
        public ulong?[] ReadUnsignedIntegerArray() { return ReadUnsignedIntegerArray(new FEncodingOptions()); }
        public ulong?[] ReadUnsignedIntegerArray(FEncodingOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Read length
            var length = (int)Read();

            // Fetch members
            var values = new ulong?[length];
            for (var i = 0; i < length; i++) {
                values[i] = ReadUnsignedInteger(options);
            }

            return values;
        }

        public long? ReadSignedInteger() { return ReadSignedInteger(new FEncodingOptions()); }
        public long? ReadSignedInteger(FEncodingOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif
            // Fetch underlying unsigned value
            var value = ReadUnsignedInteger(options);

            // Convert to signed
            return null == value ? null : (long?)ZigZag.Decode(value.Value);
        }
        public long?[] ReadSignedIntegerArray() { return ReadSignedIntegerArray(new FEncodingOptions()); }
        public long?[] ReadSignedIntegerArray(FEncodingOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Read length
            var length = (int)Read();

            // Fetch members
            var values = new long?[length];
            for (var i = 0; i < length; i++) {
                values[i] = ReadSignedInteger(options);
            }

            return values;
        }

        [Obsolete("Support final encoding subject to variation.")]
        public decimal? ReadDecimal() { return ReadDecimal(new FEncodingOptions()); }
        [Obsolete("Support final encoding subject to variation.")]
        public decimal? ReadDecimal(FEncodingOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Handle nulls
            if (options.SupportNulls) {
                var nullHeader = Read();
                if (nullHeader == 0) {
                    return null;
                }
#if DEBUG
                if (nullHeader > 1) {
                    throw new ProtocolViolationException("Unexpected null header value.");
                }
#endif
            }

            // Reconstitute
            return new decimal(new int[] { (int)Read(), (int)Read(), (int)Read(), (int)Read() });
        }
        [Obsolete("Support final encoding subject to variation.")]
        public decimal?[] ReadDecimalArray() { return ReadDecimalArray(new FEncodingOptions()); }
        [Obsolete("Support final encoding subject to variation.")]
        public decimal?[] ReadDecimalArray(FEncodingOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Read length
            var length = (int)Read();

            // Fetch members
            var values = new decimal?[length];
            for (var i = 0; i < length; i++) {
                values[i] = ReadDecimal(options);
            }

            return values;
        }

        public bool? ReadBoolean() { return ReadBoolean(new FEncodingOptions()); }
        public bool? ReadBoolean(FEncodingOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Reconstitute
            return options.SupportNulls ? options.NullableBooleanMap[Read()] : options.BooleanMap[Read()];
        }
        public bool?[] ReadBooleanArray() { return ReadBooleanArray(new FEncodingOptions()); }
        public bool?[] ReadBooleanArray(FEncodingOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Read length
            var length = (int)Read();

            // Fetch members
            var values = new bool?[length];
            for (var i = 0; i < length; i++) {
                values[i] = ReadBoolean(options);
            }

            return values;
        }

        public DateTime? ReadDateTime() { return ReadDateTime(new FEncodingOptions()); }
        public DateTime? ReadDateTime(FEncodingOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Read underlying value
            var value = Read();

            // Handle nulls
            if (options.SupportNulls) {
                if (0 == value) {
                    return null;
                } else {
                    value--;
                }
            }

            // Reconstitute
            return new DateTime(options.DateTimeMinimum.Ticks + (long)value * options.DateTimeAccuracy.Ticks);
        }
        public DateTime?[] ReadDateTimeArray() { return ReadDateTimeArray(new FEncodingOptions()); }
        public DateTime?[] ReadDateTimeArray(FEncodingOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Read length
            var length = (int)Read();

            // Fetch members
            var values = new DateTime?[length];
            for (var i = 0; i < length; i++) {
                values[i] = ReadDateTime(options);
            }

            return values;
        }

        public TimeSpan? ReadTime() { return ReadTime(new FEncodingOptions()); }
        public TimeSpan? ReadTime(FEncodingOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Read underlying value
            var value = Read();

            // Handle nulls
            if (options.SupportNulls) {
                if (0 == value) {
                    return null;
                } else {
                    value--;
                }
            }

            // Reconstitute
            return new TimeSpan(options.TimeMinimum.Ticks + (long)value * options.TimeAccuracy.Ticks);
        }
        public TimeSpan?[] ReadTimeSpanArray() { return ReadTimeSpanArray(new FEncodingOptions()); }
        public TimeSpan?[] ReadTimeSpanArray(FEncodingOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Read length
            var length = (int)Read();

            // Fetch members
            var values = new TimeSpan?[length];
            for (var i = 0; i < length; i++) {
                values[i] = ReadTime(options);
            }

            return values;
        }

        public IPAddress ReadIPAddress() { return ReadIPAddress(new FEncodingOptions()); }
        public IPAddress ReadIPAddress(FEncodingOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Read header
            var header = (int)Read();

            // Handle nulls
            if (options.SupportNulls) {
                if (0 == header) {
                    return null;
                }
            }

            // Fetch members
            var components = new byte[header];
            for (var i = 0; i < header; i++) {
                components[i] = (byte)Read();
            }

            // Reconstitute
            return new IPAddress(components);
        }
        public IPAddress[] ReadIPAddressArray() { return ReadIPAddressArray(new FEncodingOptions()); }
        public IPAddress[] ReadIPAddressArray(FEncodingOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Read length
            var length = (int)Read();

            // Fetch members
            var values = new IPAddress[length];
            for (var i = 0; i < length; i++) {
                values[i] = ReadIPAddress(options);
            }

            return values;
        }

        public string ReadString() { return ReadString(new FEncodingOptions()); }
        public string ReadString(FEncodingOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Read header
            var header = (int)Read();

            // Handle nulls
            if (options.SupportNulls) {
                if (0 == header) {
                    return null;
                } else {
                    header--;
                }
            }

            // Read components
            var components = new char[header];
            for (var i = 0; i < header; i++) {
                components[i] = (char)Read();
            }

            // Reconstitute
            return new string(components);
        }

        private ulong Read() {
            return Symbols.ReadMoveNext();
        }
        private ulong[] Read(int count) {
            var values = new ulong[count];
            for (var i = 0; i < count; i++) {
                values[i] = Read();
            }

            return values;
        }
    }
}
