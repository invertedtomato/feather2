using ThreePlay.IO.Zero.Buffers;

namespace ThreePlay.IO.Zero {
    public interface ICodec {
        /// <summary>
        /// Get a keep-alive buffer for sending.
        /// </summary>
        /// <returns></returns>
        Buffer<byte> GetKeepAlive();

        /// <summary>
        /// Instantiate a new writer.
        /// </summary>
        /// <returns></returns>
        IEncoder GetWriter();

        /// <summary>
        /// Encode given writer into a buffer.
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        Buffer<byte> Encode(IEncoder writer);

        /// <summary>
        /// Decode given buffer, ignoring any keep-alives.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        IDecoder Decode(Buffer<byte> buffer);
    }
}