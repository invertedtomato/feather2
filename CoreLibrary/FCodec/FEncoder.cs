using System;
using System.Net;
using ThreePlay.IO.Zero.Buffers;

namespace ThreePlay.IO.Zero.FCodec {
    public class FEncoder : IEncoder {

        private const int InitialSymbolCapacity = 8; // TODO: expose as option?



        public static Buffer<byte> Encode(Buffer<ulong> symbols) {
#if DEBUG
            if (null == symbols) {
                throw new ArgumentNullException("symbols");
            }
#endif

            var current = new BitBuffer();

            // Handle blank/keep-alive requests
            if (symbols.Count == 0) {
                throw new InvalidOperationException("Attempt to send payload without any parameters. Payloads must have at least one parameter.");
            }

            // Allocate buffer
            var buffer = new Buffer<byte>((symbols.Count + 1) * 12);

            // Encode set length
            EncodeSymbol(current, buffer, (ulong)symbols.Count - 1);

            // Encode symbols
            foreach (var symbol in symbols) {
#if DEBUG
                if (symbol > F.MaxValue) {
                    throw new ArgumentOutOfRangeException("Exceeded FCodec's maximum supported symbol value of " + F.MaxValue + ".", "symbols");
                }
#endif
                EncodeSymbol(current, buffer, symbol);
            }

            // Flush bit buffer
            if (current.IsDirty) {
                buffer.Write(current.Clear());
            }

            return buffer;
        }

        private static void EncodeSymbol(BitBuffer current, Buffer<byte> buffer, ulong symbol) {
            // Offset for zero
            var value = symbol + 1;

            // #1 Find the largest Fibonacci number equal to or less than N; subtract this number from N, keeping track of the remainder.
            // #3 Repeat the previous steps, substituting the remainder for N, until a remainder of 0 is reached.
            bool[] map = null;
            for (var fibIdx = F.Lookup.Length - 1; fibIdx >= 0; fibIdx--) {
                // #2 If the number subtracted was the ith Fibonacci number F(i), put a 1 in place i−2 in the code word(counting the left most digit as place 0).
                if (value >= F.Lookup[fibIdx]) {
                    // Detect if this is the largest fib and store
                    if (null == map) {
                        map = new bool[fibIdx + 1];
                    }

                    // Write to map
                    map[fibIdx] = true;

                    // Deduct Fibonacci number from value
                    value -= F.Lookup[fibIdx];
                }
            }

            // Output the bits of the map in reverse order
            foreach (var bit in map) {
                if (current.Append(bit)) {
                    buffer.Write(current.Clear());
                }
            }

            // #4 Place an additional 1 after the rightmost digit in the code word.
            if (current.Append(true)) {
                buffer.Write(current.Clear());
            }
        }


        
        public Buffer<ulong> Symbols = new Buffer<ulong>(InitialSymbolCapacity);

        public void WriteUnsignedInteger(ulong? value) { WriteUnsignedInteger(value, new FEncodingOptions()); }
        public void WriteUnsignedInteger(ulong? value, FEncodingOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Handle null support
            ulong offset = 0;
            if (options.SupportNulls) {
                if (value.HasValue) {
                    offset = 1;
                } else {
                    Append(0);
                    return;
                }
            } else if (!value.HasValue) {
                throw new ArgumentNullException("Nulls not supported with current options.", "value");
            }

            // Handle arithmetic minimization
#if DEBUG
            if (value.Value < options.UnsignedIntegerMinimum) {
                throw new ArgumentOutOfRangeException("Cannot be below UnsignedIntegerMinimum option (" + options.UnsignedIntegerMinimum + ")", "value");
            }
#endif

            // Write symbol
            Append((value.Value - options.UnsignedIntegerMinimum) / options.IntegerAccuracy + offset);
        }
        public void WriteUnsignedIntegerArray(ulong?[] values) { WriteUnsignedIntegerArray(values, new FEncodingOptions()); }
        public void WriteUnsignedIntegerArray(ulong?[] values, FEncodingOptions options) {
#if DEBUG
            if (null == values) {
                throw new ArgumentNullException("value");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Write length
            Append((ulong)values.Length);

            // Write each value
            foreach (var value in values) {
                WriteUnsignedInteger(value, options); // TODO: Not strictly correct - if an exception occurs the payload will become corrupted
            }
        }

        public void WriteSignedInteger(long? value) { WriteSignedInteger(value, new FEncodingOptions()); }
        public void WriteSignedInteger(long? value, FEncodingOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // ZigZag encode
            if (value.HasValue) {
                WriteUnsignedInteger(ZigZag.Encode(value.Value), options);
            } else {
                WriteUnsignedInteger(null, options);
            }
        }
        public void WriteSignedIntegerArray(long?[] values) { WriteSignedIntegerArray(values, new FEncodingOptions()); }
        public void WriteSignedIntegerArray(long?[] values, FEncodingOptions options) {
#if DEBUG
            if (null == values) {
                throw new ArgumentNullException("value");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Write length
            Append((ulong)values.Length);

            // Write each value
            foreach (var value in values) {
                // ZigZag encode
                if (value.HasValue) {
                    WriteUnsignedInteger(ZigZag.Encode(value.Value), options);  // TODO: Not strictly correct - if an exception occurs the payload will become corrupted
                } else {
                    WriteUnsignedInteger(null, options);  // TODO: Not strictly correct - if an exception occurs the payload will become corrupted
                }
            }
        }

        [Obsolete("Support final encoding subject to variation.")]
        public void WriteDecimal(decimal? value) { WriteDecimal(value, new FEncodingOptions()); }
        [Obsolete("Support final encoding subject to variation.")] // TODO: better encoding possible, see https://msdn.microsoft.com/en-us/library/system.decimal.getbits(v=vs.110).aspx
        public void WriteDecimal(decimal? value, FEncodingOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Handle null support
            if (options.SupportNulls) {
                if (value.HasValue) {
                    Append(1);
                } else {
                    Append(0);
                    return;
                }
            } else if (!value.HasValue) {
                throw new ArgumentNullException("Nulls not supported with current options.", "value");
            }

            // Write symbols
            foreach (ulong component in decimal.GetBits(value.Value)) {
                Append(component);
            }
        }
        [Obsolete("Support final encoding subject to variation.")]
        public void WriteDecimalArray(decimal?[] values) { WriteDecimalArray(values, new FEncodingOptions()); }
        [Obsolete("Support final encoding subject to variation.")]
        public void WriteDecimalArray(decimal?[] values, FEncodingOptions options) {
#if DEBUG
            if (null == values) {
                throw new ArgumentNullException("value");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Write length
            Append((ulong)values.Length);

            // Write each value
            foreach (var value in values) {
                WriteDecimal(value, options); // TODO: Not strictly correct - if an exception occurs the payload will become corrupted
            }
        }

        public void WriteBoolean(bool? value) { WriteBoolean(value, new FEncodingOptions()); }
        public void WriteBoolean(bool? value, FEncodingOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            if (options.SupportNulls) {
                for (var i = 0; i < options.NullableBooleanMap.Length; i++) {
                    if (value == options.NullableBooleanMap[i]) {
                        Append((ulong)i);
                        return;
                    }
                }

                throw new InvalidOperationException("EncodingOptions.NullableBooleanMap not setup correctly. Value not found.");
            } else {
                if (null == value) {
                    throw new ArgumentNullException("Nulls not supported with current options.", "value");
                }

                for (var i = 0; i < options.BooleanMap.Length; i++) {
                    if (value == options.BooleanMap[i]) {
                        Append((ulong)i);
                        return;
                    }
                }

                throw new InvalidOperationException("EncodingOptions.BooleanMap not setup correctly. Value not found.");
            }
        }
        public void WriteBooleanArray(bool?[] values) { WriteBooleanArray(values, new FEncodingOptions()); }
        public void WriteBooleanArray(bool?[] values, FEncodingOptions options) {
#if DEBUG
            if (null == values) {
                throw new ArgumentNullException("value");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Write length
            Append((ulong)values.Length);

            // Write each value
            foreach (var value in values) {
                WriteBoolean(value, options); // TODO: Not strictly correct - if an exception occurs the payload will become corrupted
            }
        }

        public void WriteDateTime(DateTime? value) { WriteDateTime(value, new FEncodingOptions()); }
        public void WriteDateTime(DateTime? value, FEncodingOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Handle null support
            ulong offset = 0;
            if (options.SupportNulls) {
                if (value.HasValue) {
                    offset = 1;
                } else {
                    Append(0);
                    return;
                }
            } else if (!value.HasValue) {
                throw new ArgumentNullException("Nulls not supported with current options.", "value");
            }

            // Handle arithmetic minimization
#if DEBUG
            if (value.Value < options.DateTimeMinimum) {
                throw new ArgumentOutOfRangeException("Cannot be below DateTimeMinimum option (" + options.DateTimeMinimum + ")", "value");
            }
#endif

            // Write symbol
            Append((ulong)(value.Value - options.DateTimeMinimum).Ticks / (ulong)options.DateTimeAccuracy.Ticks + offset);
        }
        public void WriteDateTimeArray(DateTime?[] values) { WriteDateTimeArray(values, new FEncodingOptions()); }
        public void WriteDateTimeArray(DateTime?[] values, FEncodingOptions options) {
#if DEBUG
            if (null == values) {
                throw new ArgumentNullException("value");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Write length
            Append((ulong)values.Length);

            // Write each value
            foreach (var value in values) {
                WriteDateTime(value, options); // TODO: Not strictly correct - if an exception occurs the payload will become corrupted
            }
        }

        public void WriteTime(TimeSpan? value) { WriteTime(value, new FEncodingOptions()); }
        public void WriteTime(TimeSpan? value, FEncodingOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Handle null support
            ulong offset = 0;
            if (options.SupportNulls) {
                if (value.HasValue) {
                    offset = 1;
                } else {
                    Append(0);
                    return;
                }
            } else if (!value.HasValue) {
                throw new ArgumentNullException("Nulls not supported with current options.", "value");
            }

            // Write symbol
            Append((ulong)(value.Value - options.TimeMinimum).Ticks / (ulong)options.TimeAccuracy.Ticks + offset);
        }
        public void WriteTimeSpanArray(TimeSpan?[] values) { WriteTimeSpanArray(values, new FEncodingOptions()); }
        public void WriteTimeSpanArray(TimeSpan?[] values, FEncodingOptions options) {
#if DEBUG
            if (null == values) {
                throw new ArgumentNullException("value");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Write length
            Append((ulong)values.Length);

            // Write each value
            foreach (var value in values) {
                WriteTime(value, options); // TODO: Not strictly correct - if an exception occurs the payload will become corrupted
            }
        }

        public void WriteIPAddress(IPAddress value) { WriteIPAddress(value, new FEncodingOptions()); }
        public void WriteIPAddress(IPAddress value, FEncodingOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Handle null support
            if (options.SupportNulls) {
                if (null == value) {
                    Append(0);
                    return;
                }
            } else if (null == value) {
                throw new ArgumentNullException("Nulls not supported with current options.", "value");
            }

            // Get components and write length
            var components = value.GetAddressBytes();
            Append((ulong)components.Length);

            // Write symbols
            foreach (ulong component in components) {
                Append(component);
            }
        }
        public void WriteIPAddressArray(IPAddress[] values) { WriteIPAddressArray(values, new FEncodingOptions()); }
        public void WriteIPAddressArray(IPAddress[] values, FEncodingOptions options) {
#if DEBUG
            if (null == values) {
                throw new ArgumentNullException("value");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Write length
            Append((ulong)values.Length);

            // Write each value
            foreach (var value in values) {
                WriteIPAddress(value, options); // TODO: Not strictly correct - if an exception occurs the payload will become corrupted
            }
        }

        public void WriteString(string value) { WriteString(value, new FEncodingOptions()); }
        public void WriteString(string value, FEncodingOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Handle null support
            ulong offset = 0;
            if (options.SupportNulls) {
                if (null != value) {
                    offset = 1;
                } else {
                    Append(0);
                    return;
                }
            } else if (null == value) {
                throw new ArgumentNullException("Nulls not supported with current options.", "value");
            }

            // Write length
            Append((ulong)value.Length + offset);

            // Write each character
            foreach (var c in value) {
                Append(c);
            }
        }

        /// <summary>
        /// Add a symbol to the symbol buffer
        /// </summary>
        /// <param name="symbol"></param>
        private void Append(ulong symbol) {
            // If buffer is full..
            if (Symbols.IsFull) {
                // Grow buffer
                Symbols = Symbols.Resize(Symbols.Count * 2); // TODO: make grow rate configurable
            }

            // Write symbol
            Symbols.Write(symbol);
        }

        public Buffer<byte> ToBuffer() {
            return Encode(Symbols);
        }
    }
}
