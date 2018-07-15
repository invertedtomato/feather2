using System;
using System.IO;
using System.Threading.Tasks;

namespace InvertedTomato.Net {
    public static class StreamExtensions {
        public static void Write(this Stream target, Byte[] buffer) {
            target.Write(buffer, 0, buffer.Length);
        }

        public static void Write(this Stream target, ArraySegment<Byte> buffer) {
            target.Write(buffer.Array, buffer.Offset, buffer.Count);
        }

        public static async Task WriteAsync(this Stream target, Byte[] buffer) {
            await target.WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task WriteAsync(this Stream target, ArraySegment<Byte> buffer) {
            await target.WriteAsync(buffer.Array, buffer.Offset, buffer.Count);
        }
    }
}
