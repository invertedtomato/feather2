using System;
using System.IO;
using InvertedTomato.Buffers;
using InvertedTomato;

namespace ThreePlay.IO.Feather {
    public class FeatherReader : IDisposable {
        /// <summary>
        /// If the file has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Decoding options.
        /// </summary>
        private readonly Options Options;

        /// <summary>
        /// Underlying input stream
        /// </summary>
        private readonly Stream Input;

        /// <summary>
        /// Buffer for partially read headers.
        /// </summary>
        private Buffer<byte> HeaderBuffer = new Buffer<byte>(0);

        /// <summary>
        /// Simple instantiation.
        /// </summary>
        /// <param name="input"></param>
        public FeatherReader(Stream input) : this(input, new Options()) { }

        /// <summary>
        /// Instantiate with options.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="options"></param>
        public FeatherReader(Stream input, Options options) {
#if DEBUG
            if (null == input) {
                throw new ArgumentNullException("input");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            // Store
            Input = input;
            Options = options;
        }

        /// <summary>
        /// Read next message using the given decoder.
        /// </summary>
        /// <typeparam name="TDecoder"></typeparam>
        /// <returns></returns>
        public TDecoder Read<TDecoder>() where TDecoder : IDecoder, new() {
            try {
                // Instantiate payload for reading
                var payload = new TDecoder();

                // Prepare the buffer
                if (HeaderBuffer.MaxCapacity != payload.MaxHeaderLength) {
                    HeaderBuffer = new Buffer<byte>(payload.MaxHeaderLength);
                }

                // Read first byte
                if (Input.Read(HeaderBuffer, 1) != 1) {
                    return default(TDecoder); // End of file
                }
                var length = payload.GetPayloadLength(HeaderBuffer);

                // Keep reading bytes until header if found
                var headerLength = 1;
                while (length == 0) {
                    if (HeaderBuffer.IsFull) {
                        throw new MalformedPayloadException("Length not found in the first MaxHeaderLength (" + payload.MaxHeaderLength + ") bytes.");
                    }
                    if (Input.Read(HeaderBuffer, 1) != 1) {
                        throw new MalformedPayloadException("End of file reached before header was completed.");
                    }

                    // Attempt to get length
                    length = payload.GetPayloadLength(HeaderBuffer);
                    headerLength++;
                };

                // If header was not found, abort
                if (length == -1) {
                    throw new MalformedPayloadException("Payload length not found in the first MaxHeaderLength (" + payload.MaxHeaderLength + ") bytes of payload.");
                }

                // If the payload is too large, abort
                if (length > Options.PayloadMaxSize) {
                    throw new MalformedPayloadException("Payload length (" + Math.Round((double)length / 1024 / 1024, 2) + "MB) is larger than PayloadMaxSize (" + Math.Round((double)Options.PayloadMaxSize / 1024 / 1024, 2) + "MB). ");
                }

                // Add header to buffer
                var payloadBuffer = HeaderBuffer.Resize(length);

                // Read remaining payload bytes
                var remaining = length - headerLength;
                var read = 0;
                while ((read = Input.Read(payloadBuffer, remaining)) > 0) {
                    remaining -= read;
                }
                if (remaining > 0) {
                    throw new MalformedPayloadException("End reached before all payload could be read.");
                }

                // Load into payload
                payload.LoadBuffer(payloadBuffer);

                return payload;
            } finally {
                // Reset for next read
                HeaderBuffer.Reset();
            }
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing"></param>
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

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }
    }
}
