using System;
using System.Collections;
using System.Collections.Generic;

namespace InvertedTomato.Buffers {
    public class BufferEnumerator<T> : IEnumerator<T> {
        private int Position = -1;
        private readonly ReadOnlyBuffer<T> Buffer;

        public T Current {
            get {
#if DEBUG
                if (Position == -1) {
                    throw new InvalidOperationException("Not yet moved.");
                }
                if (Position > Buffer.Used) {
                    throw new InvalidOperationException("Moved beyond end of set.");
                }
#endif
                return Buffer.Peek(Buffer.Start + Position);
            }
        }

        object IEnumerator.Current {
            get {
#if DEBUG
                if (Position == -1) {
                    throw new InvalidOperationException("Not yet moved.");
                }
                if (Position > Buffer.Used) {
                    throw new InvalidOperationException("Moved beyond end of set.");
                }
#endif
                return Buffer.Peek(Buffer.Start + Position);
            }
        }
        public bool IsDisposed { get; private set; }

        public BufferEnumerator(ReadOnlyBuffer<T> buffer) {
#if DEBUG
            if (null == buffer) {
                throw new ArgumentNullException("buffer");
            }
#endif

            // Store
            Buffer = buffer;
        }

        public bool MoveNext() {
            return ++Position < Buffer.Used;
        }

        public void Reset() {
            Position = -1;
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
