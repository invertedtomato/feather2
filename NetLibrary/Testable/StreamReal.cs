using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InvertedTomato.Testable
{
    public class StreamReal : IStream {
        private readonly Stream Underlying;

        public StreamReal(Stream underlying) {
            Underlying = underlying ?? throw new ArgumentNullException("underlying");
        }

        public long Position {get { return Underlying.Position;} set { Underlying.Position = value;}}

        public long Length {get { return Underlying.Length;}} 

        public bool CanWrite {get { return Underlying.CanWrite;}} 

        public bool CanTimeout {get { return Underlying.CanTimeout;}} 

        public bool CanSeek {get { return Underlying.CanSeek;}} 

        public bool CanRead {get { return Underlying.CanRead;}} 

        public int ReadTimeout {get { return Underlying.ReadTimeout;} set { Underlying.ReadTimeout = value;}}
        public int WriteTimeout {get { return Underlying.WriteTimeout;} set { Underlying.WriteTimeout = value;}}

        public void CopyTo(Stream destination) {            Underlying.CopyTo(destination);        }

        public void CopyTo(Stream destination, int bufferSize) { Underlying.CopyToAsync(destination, bufferSize);        }

        public Task CopyToAsync(Stream destination) {return  Underlying.CopyToAsync(destination); }

        public Task CopyToAsync(Stream destination, int bufferSize) { return Underlying.CopyToAsync(destination, bufferSize); }

        public Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) { return Underlying.CopyToAsync(destination, bufferSize, cancellationToken); }

        public void Dispose() {  Underlying.Dispose(); }

        public void Flush() { Underlying.Flush(); }

        public Task FlushAsync() { return Underlying.FlushAsync(); }

        public Task FlushAsync(CancellationToken cancellationToken) { return Underlying.FlushAsync(cancellationToken); }

        public int Read(byte[] buffer, int offset, int count) { return Underlying.Read(buffer, offset, count); }

        public Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) { return Underlying.ReadAsync(buffer, offset, count, cancellationToken); }

        public Task<int> ReadAsync(byte[] buffer, int offset, int count) { return Underlying.ReadAsync(buffer, offset, count); }

        public int ReadByte() { return Underlying.ReadByte(); }

        public long Seek(long offset, SeekOrigin origin) { return Underlying.Seek(offset, origin); }

        public void SetLength(long value) { Underlying.SetLength(value); }

        public void Write(byte[] buffer, int offset, int count) { Underlying.Write(buffer, offset, count); }

        public Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) { return Underlying.WriteAsync(buffer, offset, count, cancellationToken); }

        public Task WriteAsync(byte[] buffer, int offset, int count) { return Underlying.WriteAsync(buffer, offset, count); }

        public void WriteByte(byte value) { Underlying.WriteByte(value); }
    }
}
