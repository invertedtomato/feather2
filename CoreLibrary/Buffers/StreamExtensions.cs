using InvertedTomato.Testable.Streams;
using System;
using System.IO;

namespace InvertedTomato.Buffers {
    public static class StreamExtensions {
        public static void Write(this Stream target, ReadOnlyBuffer<byte> buffer) {
#if DEBUG
            if (null == target) {
                throw new ArgumentNullException("target");
            }
            if (null == buffer) {
                throw new ArgumentNullException("buffer");
            }
#endif

            target.Write(buffer.GetUnderlying(), buffer.Start, buffer.Used);
        }

        public static int Read(this Stream target, Buffer<byte> buffer) {
            return target.Read(buffer, int.MaxValue);
        }

        public static int Read(this Stream target, Buffer<byte> buffer, int count) {
#if DEBUG
            if (null == target) {
                throw new ArgumentNullException("target");
            }
            if (null == buffer) {
                throw new ArgumentNullException("buffer");
            }
            if (count < 0) {
                throw new ArgumentOutOfRangeException("count", "Must be at least 0.");
            }
#endif

            if (buffer.Available < count) {
                count = buffer.Available;
            }

            // Read into buffer
            var length = target.Read(buffer.GetUnderlying(), buffer.End, count);

            // Increment buffer length
            buffer.IncrementEnd(length);

            // Return number of bytes read
            return length;
        }

        public static IAsyncResult BeginRead(this IStream target, Buffer<byte> buffer, AsyncCallback callback, object state) {
#if DEBUG
            if (null == target) {
                throw new ArgumentNullException("target");
            }
            if (null == buffer) {
                throw new ArgumentNullException("buffer");
            }
#endif

            return target.BeginRead(buffer.GetUnderlying(), buffer.End, buffer.Available, callback, state);
        }

        public static IAsyncResult BeginRead(this IStream target, Buffer<byte> buffer, int maxCount, AsyncCallback callback, object state) {
#if DEBUG
            if (null == target) {
                throw new ArgumentNullException("target");
            }
            if (null == buffer) {
                throw new ArgumentNullException("buffer");
            }
#endif

            return target.BeginRead(buffer.GetUnderlying(), buffer.End, Math.Min(buffer.Available, maxCount), callback, state);
        }
    }
}
