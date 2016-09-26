using InvertedTomato.Buffers;

namespace ThreePlay.IO.Feather {
    public interface IDecoder {
        int MaxHeaderLength { get; }
        int MinHeaderLength { get; }
        ReadOnlyBuffer<byte> GetNullPayload();
        int GetPayloadLength(ReadOnlyBuffer<byte> buffer);
        void LoadBuffer(Buffer<byte> buffer);
    }
}