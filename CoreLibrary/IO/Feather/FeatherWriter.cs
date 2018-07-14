using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Messages;
using System;
using System.IO;
using System.Linq;

namespace InvertedTomato.IO.Feather {
    public class FeatherWriter<TMessage> : IDisposable where TMessage : IExportableMessage {
        /// <summary>
        /// If the file has been disposed.
        /// </summary>
        public Boolean IsDisposed { get; private set; }

        protected Stream Underlying;
        protected Boolean CascadeDispose;
        protected readonly Object Sync = new Object();
        protected readonly VLQCodec VLQ = new VLQCodec();

        public FeatherWriter(Stream underlying) : this(underlying, false) { }
        public FeatherWriter(Stream underlying, Boolean cascadeDispose) {
            if (null == underlying) {
                throw new ArgumentNullException(nameof(underlying));
            }

            // Store
            Underlying = underlying;
            CascadeDispose = cascadeDispose;
        }
        protected FeatherWriter(Boolean cascadeDispose) {
            // Store
            CascadeDispose = cascadeDispose;
        }
        
        public virtual void Write(TMessage message) {
            lock (Sync) {
                if (IsDisposed) {
                    throw new ObjectDisposedException(string.Empty);
                }

                // Extract payload
                var payload = message.Export();

                // Write length
                VLQ.CompressUnsigned(Underlying, payload.Count);

                // Write payload
                Underlying.Write(payload.Array, payload.Offset, payload.Count);
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
        public static FeatherWriter<TMessage> OpenFile(String filePath, FileAccess access) {
            var input = File.Open(filePath, FileMode.Open, access, FileShare.ReadWrite);
            return new FeatherWriter<TMessage>(input, true);
        }
    }
}
