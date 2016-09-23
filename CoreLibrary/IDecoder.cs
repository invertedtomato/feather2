using InvertedTomato.Buffers;

namespace ThreePlay.IO.Feather {
    public interface IDecoder {
        int MaxHeaderLength { get; }
        int GetPayloadLength(ReadOnlyBuffer<byte> buffer);
        bool LoadBuffer(Buffer<byte> buffer);

        // int Load(Buffer<byte> buffer); // Returns number of bytes required next
    }
}