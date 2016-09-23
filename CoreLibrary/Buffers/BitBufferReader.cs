using System;

namespace InvertedTomato.Buffers {
    /// <summary>
    /// Read bits or groups of bits from stream.
    /// </summary>
    public class BitBufferReader : IDisposable {
        /// <summary>
        /// If the reader is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Underlying input stream.
        /// </summary>
        private readonly Buffer<byte> Input;

        /// <summary>
        /// Currently buffered byte being worked on.
        /// </summary>
        private byte BufferValue;

        /// <summary>
        /// Position within the currently buffered byte. Defaulted to the end of the buffer to force a new byte to be read on the next request.
        /// </summary>
        private int BufferPosition = 8;

        /// <summary>
        /// Standard instantiation.
        /// </summary>
        /// <param name="input"></param>
        public BitBufferReader(Buffer<byte> input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            Input = input;
        }

        /// <summary>
        /// Read a set of bits. This uses ulong as a 64-bit buffer (don't think of it like an integer, think of it as a bit buffer).
        /// </summary>
        /// <param name="count">Number of bits to read, starting from the least-significant-bit (right side).</param>
        /// <returns></returns>
        public ulong Read(int count) {
            if (count > 64) {
                throw new ArgumentOutOfRangeException("Count must be between 0 and 64, not " + count + ".", "count");
            }
            if (IsDisposed) {
                throw new ObjectDisposedException("this");
            }

            ulong output = 0;

            // While there is still bits being read
            while (count > 0) {
                // If needed, load byte
                PrepareBuffer();

                // Calculate number of bits to read in this cycle - either the number of bits being requested, or the number of bits left in the buffer, whichever is less
                var chunkSize = Math.Min(count, 8 - BufferPosition);

                // Make room in output for this number of bits
                output <<= chunkSize;

                // Add bit to output
                var mask = byte.MaxValue;
                mask <<= 8 - chunkSize;
                mask >>= BufferPosition;
                output |= (ulong)(BufferValue & mask) >> 8 - chunkSize - BufferPosition;

                // Reduce number of bits remaining to be written
                count -= chunkSize;

                // Increment position in buffer by the number of bits just retrieved
                BufferPosition += chunkSize;
            }

            return output;
        }

        /// <summary>
        /// View the next bit without moving the underlying pointer.
        /// </summary>
        /// <returns></returns>
        public bool PeakBit() {
            if (IsDisposed) {
                throw new ObjectDisposedException("this");
            }

            // If needed, load byte
            PrepareBuffer();

            // Return bit from buffer
            return (BufferValue & (1 << 7 - BufferPosition)) > 0;
        }

        private void PrepareBuffer() {
            // If there are still unused bits in the buffer, do nothing
            if (BufferPosition < 8) {
                return;
            }

#if DEBUG
            // Throw exception on insane buffer position
            if (BufferPosition > 8) {
                throw new Exception("Invalid position " + BufferPosition + ". Position has been offset by an incorrect value.");
            }
#endif

            // Read next byte into buffer
            BufferValue = Input.Dequeue();

            // Reset buffer position to the start
            BufferPosition = 0;
        }

        protected virtual void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Dispose managed state (managed objects).
            }
        }

        public void Dispose() {
            Dispose(true);
        }
    }
}
