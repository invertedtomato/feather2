namespace ThreePlay.IO.Feather {
    public class Options {
        /// <summary>
        /// Maximum size to allow for encoded message.
        /// </summary>
        public int PayloadMaxSize { get; set; } = 64 * 1024 * 1024; // bytes

        /// <summary>
        /// The size the receive buffer is initially allocated as. This should be slightly larger than the average message size for best performance. Values smaller than the codec's MaxHeaderLength will have no effect.
        /// </summary>
        public int PayloadInitialBufferSize { get; set; } = 10; // bytes
    }
}
