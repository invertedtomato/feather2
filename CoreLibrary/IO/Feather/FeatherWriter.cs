using System;
using System.IO;
using InvertedTomato.IO.Buffers;
using InvertedTomato.Compression.Integers;

namespace InvertedTomato.IO.Feather {
    public class FeatherWriter<TCodec> : IDisposable where TCodec : IIntegerCodec, new() {
        /// <summary>
        /// If the file has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        private readonly Options Options;
        private readonly Stream Output;


        public FeatherWriter(Stream output) : this(output, new Options()) { }

        public FeatherWriter(Stream output, Options options) {
#if DEBUG
            if (null == output) {
                throw new ArgumentNullException("output");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Store
            Output = output;
            Options = options;
        }

        public void Write(MessageEncoder<TCodec> payload) {
#if DEBUG
            if (null == payload) {
                throw new ArgumentNullException("payload");
            }
            if (IsDisposed) {
                throw new ObjectDisposedException("this");
            }
#endif

            Write(new MessageEncoder<TCodec>[] { payload });
        }

        public void Write(MessageEncoder<TCodec>[] payloads) {
#if DEBUG
            if (null == payloads) {
                throw new ArgumentNullException("payloads");
            }
            if (IsDisposed) {
                throw new ObjectDisposedException("this");
            }
#endif

            // For each payload...
            foreach (var payload in payloads) {
#if DEBUG
                if (null == payload) {
                    throw new ArgumentNullException("payloads", "Element in array.");
                }
#endif

                // Get payload
                var payloadBuffer = payload.GetPayload();

                // Compress frame header
                var input = new Buffer<ulong>(payloadBuffer.Used);
                var frameBuffer = new Buffer<byte>(2);
                var codec = new TCodec();
                while (!codec.Compress(input, frameBuffer)) {
                    frameBuffer = frameBuffer.Resize(frameBuffer.MaxCapacity * 2);
                }
                
                lock (Output) {
                    // Write frame header
                    Output.Write(frameBuffer);

                    // Write payload
                    Output.Write(payloadBuffer);
                }
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Dispose managed state (managed objects)
            }

            // Set large fields to null
        }
        public void Dispose() {
            Dispose(true);
        }
    }
}
