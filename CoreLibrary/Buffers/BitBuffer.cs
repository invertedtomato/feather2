using System;

namespace InvertedTomato.Buffers {
    [Obsolete]
    public class BitBuffer {
        static public implicit operator byte(BitBuffer value) {
            return value.Buffer;
        }

        /// <summary>
        /// The underlying buffer.
        /// </summary>
        private byte Buffer;

        /// <summary>
        /// The position in the underlying buffer.
        /// </summary>
        private int Offset;

        public BitBuffer() { }
        public BitBuffer(byte buffer, int offset) {
#if DEBUG
            if(offset<0 || offset > 8) {
                throw new ArgumentOutOfRangeException("Offset must be between 0 and 8 inclusive.", "offset");
            }
#endif

            // Store
            Buffer = buffer;
            Offset = offset;
        }

        /// <summary>
        /// If the buffer is full.
        /// </summary>
        public bool IsFull { get { return Offset == 8; } }

        /// <summary>
        /// If the buffer contains anything.
        /// </summary>
        public bool IsDirty { get { return Offset > 0; } }

        public bool Append(bool value) {
#if DEBUG
            // Check if buffer is full
            if (Offset == 8) {
                throw new OverflowException("Buffer full");
            }
#endif
            // Write bit
            if (value) {
                Buffer |= (byte)(1 << (7 - Offset));
            }

            // Increment offset;
            Offset++;

            return Offset == 8;
        }

        public byte Clear() {
            // Copy value
            var value = Buffer;

            // Reset value
            Buffer = 0;

            // Reset offset
            Offset = 0;

            // Return value
            return value;
        }
    }
}
