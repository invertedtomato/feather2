using System;

namespace InvertedTomato.IO.Feather {
    public class MalformedPayloadException : Exception {
        public MalformedPayloadException() { }
        public MalformedPayloadException(string message) : base(message) { }
        public MalformedPayloadException(string message, Exception innerException) : base(message, innerException) { }
    }
}