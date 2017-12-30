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

        public Int64 Position {get { return Underlying.Position;} set { Underlying.Position = value;}}

        public Int64 Length {get { return Underlying.Length;}} 

        public Boolean CanWrite {get { return Underlying.CanWrite;}} 

        public Boolean CanTimeout {get { return Underlying.CanTimeout;}} 

        public Boolean CanSeek {get { return Underlying.CanSeek;}} 

        public Boolean CanRead {get { return Underlying.CanRead;}} 

        public Int32 ReadTimeout {get { return Underlying.ReadTimeout;} set { Underlying.ReadTimeout = value;}}
        public Int32 WriteTimeout {get { return Underlying.WriteTimeout;} set { Underlying.WriteTimeout = value;}}

        public void CopyTo(Stream destination) {            Underlying.CopyTo(destination);        }

        public void CopyTo(Stream destination, Int32 bufferSize) { Underlying.CopyToAsync(destination, bufferSize);        }

        public Task CopyToAsync(Stream destination) {return  Underlying.CopyToAsync(destination); }

        public Task CopyToAsync(Stream destination, Int32 bufferSize) { return Underlying.CopyToAsync(destination, bufferSize); }

        public Task CopyToAsync(Stream destination, Int32 bufferSize, CancellationToken cancellationToken) { return Underlying.CopyToAsync(destination, bufferSize, cancellationToken); }

        public void Dispose() {  Underlying.Dispose(); }

        public void Flush() { Underlying.Flush(); }

        public Task FlushAsync() { return Underlying.FlushAsync(); }

        public Task FlushAsync(CancellationToken cancellationToken) { return Underlying.FlushAsync(cancellationToken); }

        public Int32 Read(Byte[] buffer, Int32 offset, Int32 count) { return Underlying.Read(buffer, offset, count); }

        public Task<Int32> ReadAsync(Byte[] buffer, Int32 offset, Int32 count, CancellationToken cancellationToken) { return Underlying.ReadAsync(buffer, offset, count, cancellationToken); }

        public Task<Int32> ReadAsync(Byte[] buffer, Int32 offset, Int32 count) { return Underlying.ReadAsync(buffer, offset, count); }

        public Int32 ReadByte() { return Underlying.ReadByte(); }

        public Int64 Seek(Int64 offset, SeekOrigin origin) { return Underlying.Seek(offset, origin); }

        public void SetLength(Int64 value) { Underlying.SetLength(value); }

        public void Write(Byte[] buffer, Int32 offset, Int32 count) { Underlying.Write(buffer, offset, count); }

        public Task WriteAsync(Byte[] buffer, Int32 offset, Int32 count, CancellationToken cancellationToken) { return Underlying.WriteAsync(buffer, offset, count, cancellationToken); }

        public Task WriteAsync(Byte[] buffer, Int32 offset, Int32 count) { return Underlying.WriteAsync(buffer, offset, count); }

        public void WriteByte(Byte value) { Underlying.WriteByte(value); }
    }
}
