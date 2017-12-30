using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InvertedTomato.Testable {
    public interface IStream {
        Int64 Position { get; set; }
        Int64 Length { get; }
        Boolean CanWrite { get; }
        Boolean CanTimeout { get; }
        Boolean CanSeek { get; }
        Boolean CanRead { get; }
        Int32 ReadTimeout { get; set; }
        Int32 WriteTimeout { get; set; }

        void CopyTo(Stream destination);
        void CopyTo(Stream destination, Int32 bufferSize);
        Task CopyToAsync(Stream destination);
        Task CopyToAsync(Stream destination, Int32 bufferSize);
        Task CopyToAsync(Stream destination, Int32 bufferSize, CancellationToken cancellationToken);
        void Dispose();
        void Flush();
        Task FlushAsync();
        Task FlushAsync(CancellationToken cancellationToken);
        Int32 Read(Byte[] buffer, Int32 offset, Int32 count);
        Task<Int32> ReadAsync(Byte[] buffer, Int32 offset, Int32 count, CancellationToken cancellationToken);
        Task<Int32> ReadAsync(Byte[] buffer, Int32 offset, Int32 count);
        Int32 ReadByte();
        Int64 Seek(Int64 offset, SeekOrigin origin);
        void SetLength(Int64 value);
        void Write(Byte[] buffer, Int32 offset, Int32 count);
        Task WriteAsync(Byte[] buffer, Int32 offset, Int32 count, CancellationToken cancellationToken);
        Task WriteAsync(Byte[] buffer, Int32 offset, Int32 count);
        void WriteByte(Byte value);
    }
}
