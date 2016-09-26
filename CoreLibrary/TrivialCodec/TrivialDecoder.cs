using InvertedTomato.Buffers;
using ThreePlay.IO.Feather;

namespace InvertedTomato.Feather.TrivialCodec {
    public sealed class TrivialDecoder : IDecoder {
        public Buffer<byte> SymbolBuffer;

        public int MaxHeaderLength { get { return 1; } }

        public int GetPayloadLength(ReadOnlyBuffer<byte> buffer) {
            if (buffer.Used < 1) {
                return -1;
            }

            return buffer.Peek();
        }

        public void LoadBuffer(Buffer<byte> buffer) {
            SymbolBuffer = buffer;
        }
    }
}
