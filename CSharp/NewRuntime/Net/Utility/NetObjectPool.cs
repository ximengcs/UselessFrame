using System;
using System.Threading;
using System.Collections.Concurrent;

namespace UselessFrame.Net
{
    internal class NetObjectPool<T> where T : class, INetPoolObject, new()
    {
        private readonly ConcurrentQueue<T> _pool = new ConcurrentQueue<T>();
        private readonly int _maxSize;
        private int _count;
        private bool _disposed;

        public NetObjectPool(int maxSize = 128)
        {
            _maxSize = Math.Max(1, maxSize);
        }

        public T Require()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(NetObjectPool<T>));

            if (_pool.TryDequeue(out var item))
            {
                Interlocked.Decrement(ref _count);
                return item;
            }

            return new T();
        }

        public void Release(T item)
        {
            if (_disposed || item == null) return;

            item.Reset();
            DisposeItem(item);

            if (Interlocked.Increment(ref _count) > _maxSize)
            {
                Interlocked.Decrement(ref _count);
                DisposeItem(item);
                return;
            }

            _pool.Enqueue(item);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            while (_pool.TryDequeue(out var item))
            {
                DisposeItem(item);
            }
        }

        private void DisposeItem(T item)
        {
            Interlocked.Decrement(ref _count);
        }

        public int Count => _count;

        public int MaxSize => _maxSize;
    }
}
