using System;
using System.IO;
using InvertedTomato.IO.Buffers;
using InvertedTomato.Compression.Integers;

namespace InvertedTomato.IO.Feather {
    public class FeatherReader<TMessage> : IDisposable where TMessage : IMessage, new() {
        /// <summary>
        /// If the file has been disposed.
        /// </summary>
        public Boolean IsDisposed { get; private set; }
        
        private readonly Boolean OwnsInput;

        /// <summary>
        /// Underlying input stream
        /// </summary>
        private readonly Stream Input;

        /// <summary>
        /// Simple instantiation.
        /// </summary>
        /// <param name="input"></param>
        public FeatherReader(Stream input) : this(input, false) { }
        
        public FeatherReader(Stream input, Boolean ownsInput) {
#if DEBUG
            if (null == input) {
                throw new ArgumentNullException("input");
            }
#endif

            // Store
            Input = input;
            OwnsInput = ownsInput;
        }

        /// <summary>
        /// Read next message
        /// </summary>
        public TMessage Read() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(Boolean disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Dispose managed state (managed objects)
                if(OwnsInput) {
                    Input?.Dispose();
                }
            }

            // Set large fields to null
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }

        public static FeatherReader<TMessage> OpenFile(string filePath) {
            var input = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return new FeatherReader<TMessage>(input);
        }
    }
}
