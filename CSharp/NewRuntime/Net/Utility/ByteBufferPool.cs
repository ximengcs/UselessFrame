
using System;
using System.Buffers;
using System.Drawing;

namespace UselessFrame.Net
{
    public class ByteBufferPool : IDisposable
    {
        private bool _disposed;
        private ArrayPool<byte> _bufferPool;

        public ByteBufferPool()
        {
            _bufferPool = ArrayPool<byte>.Create(1024 * 1024 * 10, 128);
        }

        public byte[] Require(int count)
        {
            Console.WriteLine($"require {count}");
            return _bufferPool.Rent(count);
        }

        public void Release(byte[] data)
        {
            Console.WriteLine($"Release {data.Length}");
            _bufferPool.Return(data);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _bufferPool = null;
        }
    }
}
