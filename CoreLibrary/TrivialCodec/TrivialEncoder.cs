using InvertedTomato.Buffers;
using System;
using ThreePlay.IO.Feather;

namespace InvertedTomato.Feather.TrivialCodec {
    public sealed class TrivialEncoder : IEncoder {
        private readonly Buffer<byte> SymbolBuffer;

        public TrivialEncoder() {
            throw new NotSupportedException();
        }
        public TrivialEncoder(byte[] payload) {
            if (null == payload) {
                throw new ArgumentNullException("payload");
            }
            if (payload.Length > byte.MaxValue) {
                throw new ArgumentException("Payload can be no more than " + byte.MaxValue + "bytes.");
            }

            SymbolBuffer = new Buffer<byte>(payload.Length + 1);
            SymbolBuffer.Enqueue((byte)payload.Length);
            SymbolBuffer.EnqueueArray(payload);
        }

        public ReadOnlyBuffer<byte> GetBuffer() {
            return SymbolBuffer;
        }
    }
}
