using System;
using ThreePlay.IO.Zero.Buffers;

namespace ThreePlay.IO.Zero.FCodec {
    public sealed class F {
        /// <summary>
        /// The maximum value of a symbol this codec can support.
        /// </summary>
        public const ulong MaxValue = ulong.MaxValue - 1;

        /// <summary>
        /// Lookup table of Fibonacci numbers that can fit in a ulong.
        /// </summary>
        public static readonly ulong[] Lookup = new ulong[92];

        /// <summary>
        /// The most significant bit in a byte.
        /// </summary>
        private const byte MSB = 0x80;

        static F() {
            // Compute all Fibonacci numbers that can fit in a ulong.
            Lookup[0] = 1;
            Lookup[1] = 2;
            for (var i = 2; i < Lookup.Length; i++) {
                Lookup[i] = Lookup[i - 1] + Lookup[i - 2];
            }
        }

        public static ulong DecodeNext(Buffer<byte> buffer) {
            // State of the last bit while decoding.
            var lastBit = false;

            // Current symbol being decoded.
            ulong symbol = 0;

            // Next Fibonacci number to test.
            var nextFibIndex = 0;

            // Load first byte
            var input = buffer.SubOffset == 8 ? buffer.ReadMoveNext() : buffer.Read();

            // For each bit of buffer
            while (true) {
                // If bit is set...
                if (((input << buffer.SubOffset) & MSB) > 0) {
                    // If double 1 bits
                    if (lastBit) {
                        buffer.SubOffset++;
                        return symbol - 1;
                    }

                    // Add value to current symbol
                    symbol += F.Lookup[nextFibIndex];

                    // Note bit for next cycle
                    lastBit = true;
                } else {
                    // Note bit for next cycle
                    lastBit = false;
                }

                // Increment bit position
                nextFibIndex++;

#if DEBUG
                // Check for overflow
                if (nextFibIndex >= F.Lookup.Length) {
                    throw new OverflowException("Value too large to decode. Max 64bits supported.");  // TODO: Handle this so that it doesn't allow for DoS attacks!
                }
#endif

                // If we've reached the end of the byte, load next
                if (++buffer.SubOffset == 8) {
                    buffer.SubOffset = 0;
                    input = buffer.ReadMoveNext();
                }
            }
        }
    }
}
