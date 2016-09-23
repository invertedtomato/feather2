using System;
using System.Text;
using InvertedTomato.Buffers;
using ThreePlay.IO.Feather;

namespace InvertedTomato.IO.Feather.CSVCodec {
    public class CSVDecoder : IDecoder {
        public static Buffer<string> Decode(Buffer<byte> buffer) {
#if DEBUG
            if (null == buffer) {
                throw new ArgumentNullException("buffer");
            }
#endif

            var isQuoted = false;
            var isEscaped = false;
            var symbol = new StringBuilder();
            var symbols = new Buffer<string>(8); // TODO: Allow variation?

            // Loop all bytes in buffer // TODO: Not UTF8 compliant??
            while (!buffer.IsEmpty) {
                var c = (char)buffer.Dequeue();

                // If quoted...
                if (isQuoted) {
                    // If escaped...
                    if (isEscaped) {
                        // Append character to symbol
                        symbol.Append(c);

                        // Cancel escape
                        isEscaped = false;
                    } else
                        if (c == '\\') {
                        // Note that the next character is escaped
                        isEscaped = true;
                    } else if (c == '"') {
                        // Note the end of a quoted block
                        isQuoted = false;
                    } else {
                        // Append character to symbol
                        symbol.Append(c);
                    }
                } else if (c == '\\') {
                    // Note that the next character is escaped
                    isEscaped = true;
                } else if (c == '"') {
                    // Note the start of a quoted block
                    isQuoted = true;
                } else if (c == ',') { // End of symbol
                                       // Flush symbol
                    symbols.Enqueue(symbol.ToString());
                    symbol.Clear();
                } else if (c == '\n') { // End of symbol and payload
                    // Flush symbol
                    symbols.Enqueue(symbol.ToString());
                    return symbols;
                } else {
                    // Append character to symbol
                    symbol.Append(c);
                }
            }
            
            return null;
        }

        public static int DecodeLength(ReadOnlyBuffer<byte> buffer) {
#if DEBUG
            if (null == buffer) {
                throw new ArgumentNullException("buffer");
            }
#endif

            var isQuoted = false;
            var isEscaped = false;

            // Loop all bytes in buffer // TODO: Not UTF8 compliant??
            var count = 0;
            foreach (char c in buffer) {
                count++;

                // If quoted...
                if (isQuoted) {
                    // If escaped...
                    if (isEscaped) {
                        // Cancel escape
                        isEscaped = false;
                    } else
                        if (c == '\\') {
                        // Note that the next character is escaped
                        isEscaped = true;
                    } else if (c == '"') {
                        // Note the end of a quoted block
                        isQuoted = false;
                    }
                } else if (c == '\\') {
                    // Note that the next character is escaped
                    isEscaped = true;
                } else if (c == '"') {
                    // Note the start of a quoted block
                    isQuoted = true;
                } else if (c == '\n') { // End of symbol and payload
                    return count;
                }
            }

            return -1;
        }

        private Buffer<string> Symbols;

        public int MaxHeaderLength { get { return 1 * 1024 * 1025; } }

        public CSVDecoder() { }
        public CSVDecoder(string[] symbols) {
            if (null == symbols) {
                throw new ArgumentNullException("symbols");
            }

            Symbols = new Buffer<string>(symbols);
        }

        public long ReadInteger() {
            return long.Parse(Symbols.Dequeue());
        }

        public decimal ReadDecimal() {
            return decimal.Parse(Symbols.Dequeue());
        }

        public bool ReadBoolean() {
            return bool.Parse(Symbols.Dequeue());
        }

        public DateTime ReadDateTime() {
            return DateTime.Parse(Symbols.Dequeue());
        }

        public TimeSpan ReadTime() {
            return TimeSpan.Parse(Symbols.Dequeue());
        }

        public string ReadString() {
            return Symbols.Dequeue();
        }

        public bool LoadBuffer(Buffer<byte> buffer) {
            var symbols = Decode(buffer);
            if (null == symbols) {
                return false;
            }
            Symbols = symbols;
            return true;
        }

        public int GetPayloadLength(ReadOnlyBuffer<byte> buffer) {
            return DecodeLength(buffer);
        }
    }
}