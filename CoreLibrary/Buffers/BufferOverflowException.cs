using System;

namespace InvertedTomato.Buffers {
    [Serializable]
    public class BufferOverflowException : Exception {
        public BufferOverflowException() { }
        public BufferOverflowException(string message) : base(message) { }
        public BufferOverflowException(string message, Exception inner) : base(message, inner) { }
        protected BufferOverflowException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}