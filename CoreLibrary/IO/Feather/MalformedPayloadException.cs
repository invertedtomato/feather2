using System;

namespace InvertedTomato.IO.Feather {
    public class MalformedPayloadException : Exception {
        public MalformedPayloadException() { }
        public MalformedPayloadException(String message) : base(message) { }
        public MalformedPayloadException(String message, Exception innerException) : base(message, innerException) { }
    }
}