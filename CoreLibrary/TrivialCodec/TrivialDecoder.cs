using InvertedTomato.Buffers;
using ThreePlay.IO.Feather;

namespace InvertedTomato.Feather.TrivialCodec {
    public sealed class TrivialDecoder : IDecoder {
        public byte[] Symbols;

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
            // Burn header
            if(buffer.Dequeue() != buffer.Used) {
                throw new MalformedPayloadException();
            }

            // Store
            Symbols = buffer.ToArray();
        }

        public ReadOnlyBuffer<byte> GetNullPayload() {
            return new Buffer<byte>(new byte[] { 0 });
        }
    }
}
