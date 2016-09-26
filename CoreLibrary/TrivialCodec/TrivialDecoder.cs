using InvertedTomato.Buffers;
using ThreePlay.IO.Feather;

namespace InvertedTomato.Feather.TrivialCodec {
    public sealed class TrivialDecoder : IDecoder {
        public Buffer<byte> SymbolBuffer;

        public int MinHeaderLength { get { return 1; } }
        public int MaxHeaderLength { get { return 1; } }

        public int GetPayloadLength(ReadOnlyBuffer<byte> buffer) {
            // Abort if there is insufficient length
            if (buffer.Used < 1) {
                return 0;
            }

            // Return length (including header)
            return buffer.Peek() + 1;
        }

        public void LoadBuffer(Buffer<byte> buffer) {
            // Store
            SymbolBuffer = buffer;

            // Burn header
            if(SymbolBuffer.Dequeue() != SymbolBuffer.Used) {
                throw new MalformedPayloadException();
            }
        }

        public ReadOnlyBuffer<byte> GetNullPayload() {
            return new Buffer<byte>(new byte[] { 0 });
        }
    }
}
