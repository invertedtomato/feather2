using System;
using System.IO;
using InvertedTomato.IO.Buffers;
using InvertedTomato.Compression.Integers;

namespace InvertedTomato.IO.Feather {
    public class FeatherWriter<TMessage> : IDisposable where TMessage : IMessage, new() {
        /// <summary>
        /// If the file has been disposed.
        /// </summary>
        public Boolean IsDisposed { get; private set; }
        
        private readonly Stream Output;
        private readonly Boolean OwnsOutput;

        public FeatherWriter(Stream output) : this(output, false) { }

        public FeatherWriter(Stream output, Boolean ownsOutput) {
#if DEBUG
            if (null == output) {
                throw new ArgumentNullException("output");
            }
#endif

            // Store
            Output = output;
            OwnsOutput = ownsOutput;
        }

        public void Write(TMessage message) {
#if DEBUG
            if (null == message) {
                throw new ArgumentNullException(nameof(message);
            }
#endif
            if(IsDisposed) {
                throw new ObjectDisposedException("this");
            }

            throw new NotImplementedException();
        }
        

        protected virtual void Dispose(Boolean disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Dispose managed state (managed objects)
                if(OwnsOutput) {
                    Output?.Dispose();
                }
            }

            // Set large fields to null
        }
        public void Dispose() {
            Dispose(true);
        }
    }
}
