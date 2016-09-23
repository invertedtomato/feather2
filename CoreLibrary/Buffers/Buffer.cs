﻿using System;

namespace InvertedTomato.Buffers {
    public class Buffer<T> : ReadOnlyBuffer<T> {
        /// <summary>
        /// Create a new buffer initialized to the given length.
        /// </summary>
        /// <param name="maxCapacity"></param>
        public Buffer(int maxCapacity) : base(new T[maxCapacity], 0, 0) {
#if DEBUG
            if (maxCapacity < 0) {
                throw new ArgumentOutOfRangeException("Must be at least 0.", "maxCapacity");
            }
#endif
        }

        /// <summary>
        /// Create a buffer from a preexisting array.
        /// </summary>
        /// <param name="underlying"></param>
        public Buffer(T[] underlying) : base(underlying, 0, underlying.Length) { }

        /// <summary>
        /// Create a buffer from a preexisting array.
        /// </summary>
        /// <param name="underlying"></param>
        /// <param name="count"></param>
        public Buffer(T[] underlying, int count) : base(underlying, 0, count) { }

        /// <summary>
        /// Create a buffer from a preexisting array.
        /// </summary>
        /// <param name="underlying"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public Buffer(T[] underlying, int offset, int count) : base(underlying, offset, count) { }

        /// <summary>
        /// Add a value to the buffer and increment End.
        /// </summary>
        /// <param name="value"></param>
        public void Enqueue(T value) {
#if DEBUG
            if (IsFull) {
                throw new BufferOverflowException("Buffer is already full.");
            }
#endif

            Underlying[End++] = value;
        }

        /// <summary>
        /// Append a new buffer to this buffer and increment End.
        /// </summary>
        /// <param name="buffer"></param>
        public void Enqueue(ReadOnlyBuffer<T> buffer) {
#if DEBUG
            if (null == buffer) {
                throw new ArgumentNullException("buffer");
            }
            if (buffer.Used > Available) {
                throw new BufferOverflowException("Insufficient space in buffer. " + Available + " available, but " + buffer.Used + " needed.");
            }
#endif

            Array.Copy(buffer.GetUnderlying(), buffer.Start, Underlying, End - 1, buffer.Used);
            End += buffer.Used;
        }

        /// <summary>
        /// Return the next value from the buffer and increment Start.
        /// </summary>
        /// <returns></returns>
        public T Dequeue() {
#if DEBUG
            if (IsEmpty) {
                throw new BufferOverflowException("Buffer is empty.");
            }
#endif

            return Underlying[Start++];
        }

        /// <summary>
        /// Get the next value from the buffer using try pattern.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool TryDequeue(out T output) {
            if (IsEmpty) {
                output = default(T);
                return false;
            } else {
                output = Underlying[Start++];
                return true;
            }
        }

        /// <summary>
        /// Reset the pointers ready for buffer re-use.
        /// </summary>
        /// <returns></returns>
        public void Reset() {
            Start = 0;
            End = 0;
        }

        public void IncrementEnd(int count) {
#if DEBUG
            if (count > Available) {
                throw new BufferOverflowException("Insufficient space in buffer.");
            }
#endif

            End += count;
        }

        public ReadOnlyBuffer<T> AsReadOnly() {
            return this;
        }
    }
}
