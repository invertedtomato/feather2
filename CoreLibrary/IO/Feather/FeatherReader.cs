using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Messages;
using System;
using System.IO;
using System.Linq;

namespace InvertedTomato.IO.Feather {
    public class FeatherReader<TMessage> : IDisposable where TMessage : IImportableMessage, new() {
        /// <summary>
        /// If the file has been disposed.
        /// </summary>
        public Boolean IsDisposed { get; private set; }

        protected Stream Underlying;
        protected Boolean CascadeDispose;
        protected readonly Object Sync = new Object();
        protected readonly VLQCodec VLQ = new VLQCodec();

        public FeatherReader(Stream underlying) : this(underlying, false) { }
        public FeatherReader(Stream underlying, Boolean cascadeDispose) {
            if (null == underlying) {
                throw new ArgumentNullException(nameof(underlying));
            }

            // Store
            Underlying = underlying;
            CascadeDispose = cascadeDispose;
        }
        protected FeatherReader(Boolean cascadeDispose) {
            // Store
            CascadeDispose = cascadeDispose;
        }

        public virtual TMessage Read() {
            lock (Sync) {
                if (IsDisposed) {
                    throw new ObjectDisposedException(string.Empty);
                }

                // Read length
                var length = VLQ.DecompressUnsigned(Underlying, 1).Single();

                // Read payload
                var payload = new Byte[length];
                Underlying.Read(payload, 0, payload.Length);

                // Create message
                var message = default(TMessage);
                message.Import(new ArraySegment<byte>(payload));

                return message;
            }
        }

        protected virtual void Dispose(Boolean disposing) {
            lock (Sync) {
                if (IsDisposed) {
                    return;
                }
                IsDisposed = true;

                if (disposing) {
                    // Dispose managed state (managed objects).
                    if (CascadeDispose) {
                        Underlying?.Dispose();
                    }
                }

                // Set large fields to null.
            }
        }

        public void Dispose() {
            Dispose(true);
        }

        /// <summary>
        /// Vanity method to open binary file.
        /// </summary>
        public static FeatherReader<TMessage> OpenFile(String filePath, FileAccess access) {
            var input = File.Open(filePath, FileMode.Open, access, FileShare.ReadWrite);
            return new FeatherReader<TMessage>(input, true);
        }
    }
}
