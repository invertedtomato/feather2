using InvertedTomato.Buffers;

namespace ThreePlay.IO.Feather {
    public interface IEncoder {
        ReadOnlyBuffer<byte> ToBuffer();
    }
}