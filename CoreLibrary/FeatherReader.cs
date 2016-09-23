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

        private readonly Options Options;
        private readonly Stream Input;

        private Buffer<byte> LengthBuffer = new Buffer<byte>(0);

        public FeatherReader(Stream input) : this(input, new Options()) { }

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

        public TDecoder Read<TDecoder>() where TDecoder : IDecoder, new() {
            try {
                // Instantiate payload for reading
                var payload = new TDecoder();

                // Prepare the buffer
                if (LengthBuffer.MaxCapacity != payload.MaxHeaderLength) {
                    LengthBuffer = new Buffer<byte>(payload.MaxHeaderLength);
                }

                // Read first byte
                if (Input.Read(LengthBuffer, 1) != 1) {
                    return default(TDecoder); // End of file
                }
                var length = payload.GetPayloadLength(LengthBuffer);

                // Keep reading bytes until header if found
                var headerLength = 1;
                while (length == -1) {
                    if (LengthBuffer.IsFull) {
                        throw new MalformedPayloadException("Length not found in the first MaxHeaderLength (" + payload.MaxHeaderLength + ") bytes.");
                    }
                    if (Input.Read(LengthBuffer, 1) != 1) {
                        throw new MalformedPayloadException("End of file reached before header was completed.");
                    }

                    // Attempt to get length
                    length = payload.GetPayloadLength(LengthBuffer);
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
                var buffer = LengthBuffer.Resize(length);

                // Read remaining payload bytes
                var remaining = length - headerLength;
                var read = 0;
                while ((read = Input.Read(buffer, remaining)) > 0) {
                    remaining -= read;
                }
                if (remaining > 0) {
                    throw new MalformedPayloadException("End reached before all payload could be read.");
                }

                // Load into payload
                if (!payload.LoadBuffer(buffer)) {
                    throw new MalformedPayloadException("Decoder rejected payload as invalid.");
                }

                return payload;
            } finally {
                // Reset for next read
                LengthBuffer.Reset();
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
