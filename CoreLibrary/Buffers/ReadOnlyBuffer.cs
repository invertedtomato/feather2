using System;
using System.Collections;
using System.Collections.Generic;

namespace InvertedTomato.Buffers {
    public class ReadOnlyBuffer<T> : IEnumerable<T> {
        /// <summary>
        /// The position of the first used byte.
        /// </summary>
        public int Start { get; protected set; }

        /// <summary>
        /// The position of the last used byte.
        /// </summary>
        public int End { get; protected set; }

        /// <summary>
        /// The number of values in the buffer.
        /// </summary>
        public int Used { get { return End - Start; } }

        /// <summary>
        /// The number of additional values that could be added to the buffer.
        /// </summary>
        public int Available { get { return Underlying.Length - End; } }

        /// <summary>
        /// The maximum number of values that could be added in optimal circumstances.
        /// </summary>
        public int MaxCapacity { get { return Underlying.Length; } }

        //public bool IsDirty { get { return Used > 0; } }
        /// <summary>
        /// Is the buffer unable to accept any additional values.
        /// </summary>
        public bool IsFull { get { return Available == 0; } }

        /// <summary>
        /// Is the buffer empty and unable to provide any more values.
        /// </summary>
        public bool IsEmpty { get { return Used == 0; } }

        /// <summary>
        /// The underlying buffer array.
        /// </summary>
        protected readonly T[] Underlying;


        /// <summary>
        /// Create a buffer from a preexisting array.
        /// </summary>
        /// <param name="underlying"></param>
        public ReadOnlyBuffer(T[] underlying) : this(underlying, 0, underlying.Length) { }

        /// <summary>
        /// Create a buffer from a preexisting array.
        /// </summary>
        /// <param name="underlying"></param>
        /// <param name="count"></param>
        public ReadOnlyBuffer(T[] underlying, int count) : this(underlying, 0, count) { }

        /// <summary>
        /// Create a buffer from a preexisting array.
        /// </summary>
        /// <param name="underlying"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public ReadOnlyBuffer(T[] underlying, int offset, int count) {
#if DEBUG
            if (null == underlying) {
                throw new ArgumentNullException("value");
            }
            if (offset < 0 || offset > underlying.Length) {
                throw new ArgumentOutOfRangeException("Must be at least 0 and no more than the underlying length.", "offset");
            }
            if (count < 0) {
                throw new ArgumentOutOfRangeException("Must be at least 0.", "count");
            }
#endif

            // Store
            if (count <= underlying.Length) {
                Underlying = underlying;
                Start = offset;
                End = Start + count;
            } else {
                Underlying = new T[count];
                Array.Copy(underlying, offset, Underlying, 0, underlying.Length);
                Start = 0;
                End = underlying.Length;
            }
        }


        /// <summary>
        /// Return the next value from the buffer without incrementing Start.
        /// </summary>
        /// <returns></returns>
        public T Peek() {
#if DEBUG
            if (IsEmpty) {
                throw new BufferOverflowException("Buffer is empty.");
            }
#endif

            return Underlying[Start];
        }

        /// <summary>
        /// Get the next value from the buffer using try pattern, without moving Start.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool TryPeek(out T output) {
            if (IsEmpty) {
                output = default(T);
                return false;
            } else {
                output = Underlying[Start];
                return true;
            }
        }

        /// <summary>
        /// Return a specific value from the buffer without changing Start.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public T Peek(int position) {
#if DEBUG
            if (position < Start || position >= End) {
                throw new BufferOverflowException("No value in given position.");
            }
#endif

            return Underlying[position];
        }

        /// <summary>
        /// Return a resized copy of the buffer.
        /// </summary>
        /// <param name="maxCapacity"></param>
        /// <returns></returns>
        public Buffer<T> Resize(int maxCapacity) {
            // Check capacity is sufficient
            if (maxCapacity < Used) {
                throw new BufferOverflowException("Length is smaller than the number of used bytes (" + Used + ").");
            }

            // Create new underlying
            var underlying = new T[maxCapacity];
            Array.Copy(Underlying, Start, underlying, 0, Used);

            // Return new buffer
            return new Buffer<T>(underlying, Used);
        }

        /// <summary>
        /// Return the underlying buffer array. USE WITH CAUTION.
        /// </summary>
        /// <returns></returns>
        public T[] GetUnderlying() {
            return Underlying;
        }

        /// <summary>
        /// Extract data as array.
        /// </summary>
        /// <returns></returns>
        public T[] ToArray() {
            var underlying = new T[Used];
            Array.Copy(Underlying, Start, underlying, 0, Used);

            return underlying;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return new BufferEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return new BufferEnumerator<T>(this);
        }

        public override string ToString() {
            var byteArray = Underlying as byte[];
            if (null != byteArray) {
                return BitConverter.ToString(byteArray, Start, Used);
            }

            var sb = new System.Text.StringBuilder();
            for (var i = Start; i < End; i++) {
                if (sb.Length > 0) {
                    sb.Append("-");
                }
                sb.Append(Underlying[i]);
            }

            return sb.ToString();
        }
    }
}
