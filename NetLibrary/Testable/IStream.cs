using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InvertedTomato.Testable {
    public interface IStream {
        long Position { get; set; }
        long Length { get; }
        bool CanWrite { get; }
        bool CanTimeout { get; }
        bool CanSeek { get; }
        bool CanRead { get; }
        int ReadTimeout { get; set; }
        int WriteTimeout { get; set; }

        void CopyTo(Stream destination);
        void CopyTo(Stream destination, int bufferSize);
        Task CopyToAsync(Stream destination);
        Task CopyToAsync(Stream destination, int bufferSize);
        Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken);
        void Dispose();
        void Flush();
        Task FlushAsync();
        Task FlushAsync(CancellationToken cancellationToken);
        int Read(byte[] buffer, int offset, int count);
        Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);
        Task<int> ReadAsync(byte[] buffer, int offset, int count);
        int ReadByte();
        long Seek(long offset, SeekOrigin origin);
        void SetLength(long value);
        void Write(byte[] buffer, int offset, int count);
        Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);
        Task WriteAsync(byte[] buffer, int offset, int count);
        void WriteByte(byte value);
    }
}
