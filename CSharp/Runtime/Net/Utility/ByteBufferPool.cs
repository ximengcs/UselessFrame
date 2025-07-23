
using System;
using System.Buffers;

namespace UselessFrame.Net
{
    public class ByteBufferPool : IDisposable
    {
        private bool _disposed;

        public byte[] Require(int count)
        {
            return ArrayPool<byte>.Shared.Rent(count);
        }

        public void Release(byte[] data)
        {
            ArrayPool<byte>.Shared.Return(data);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
        }
    }
}
