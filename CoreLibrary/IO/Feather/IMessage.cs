using System;

namespace InvertedTomato.IO.Feather {
    public interface IMessage {
        Byte[] ToByteArray();
        void FromByteArray(Byte[] payload);
    }
}
