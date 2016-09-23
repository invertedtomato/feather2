using System;
using System.Runtime.Serialization;

namespace ThreePlay.IO.Feather {
    [Serializable]
    public class MalformedPayloadException : Exception {
        public MalformedPayloadException() { }

        public MalformedPayloadException(string message) : base(message) { }

        public MalformedPayloadException(string message, Exception innerException) : base(message, innerException) { }

        protected MalformedPayloadException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}