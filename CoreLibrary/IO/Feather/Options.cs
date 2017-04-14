using System;

namespace InvertedTomato.IO.Feather {
    public class Options {
        /// <summary>
        /// Maximum size to allow for encoded message.
        /// </summary>
        public int PayloadMaxSize { get; set; } = 64 * 1024 * 1024; // bytes
    }
}
