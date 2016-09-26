using System;
using System.Collections.Generic;
using InvertedTomato.Buffers;
using ThreePlay.IO.Feather;

namespace InvertedTomato.IO.Feather.CSVCodec {
    public class CSVEncoder : IEncoder {
        public static Buffer<byte> Encode(List<string> symbols) {
#if DEBUG
            if (null == symbols) {
                throw new ArgumentNullException("writer");
            }
#endif

            var isEmpty = true;
            var output = new System.Text.StringBuilder();
            foreach (var value in symbols) {
                // Add prefix comma if not first symbol
                if (!isEmpty) {
                    output.Append(",");
                }
                isEmpty = false;

                // If escaping is needed...
                if (value.Contains(",") || value.Contains("\"") || value.Contains("\r") || value.Contains("\n")) {
                    // Wrap in quotes
                    output.Append("\"");
                    output.Append(value.Replace("\\", "\\\\").Replace("\"", "\\\""));
                    output.Append("\"");
                } else {
                    // Write value directly
                    output.Append(value);
                }
            }

            // Add termination
            output.Append("\n");

            // Get output and complete CSV
            var raw = System.Text.Encoding.UTF8.GetBytes(output.ToString());

            // Convert to buffer
            return new Buffer<byte>(raw);
        }


        public List<string> Symbols = new List<string>(); // TODO: Protect from external mangling

        public void WriteInteger(long value) {
            Write(value.ToString());
        }
        
        public void WriteDecimal(decimal value) {
            Write(value.ToString());
        }

        public void WriteBoolean(bool value) {
            Write(value ? "true" : "false");
        }

        public void WriteDateTime(DateTime value) {
            Write(value.ToString("u")); }

        public void WriteTime(TimeSpan value) { 
Write(value.ToString()); }

        public void WriteString(string value) {
#if DEBUG
            if (null == value) {
                throw new ArgumentNullException("value", "Nulls not supported in strings.");
            }
#endif

            Write(value);
        }

        private void Write(string value) {
#if DEBUG
            if (null == value) {
                throw new ArgumentNullException("value");
            }
#endif

            Symbols.Add(value);
        }

        public ReadOnlyBuffer<byte> GetBuffer() {
            return Encode(Symbols);
        }
    }
}
