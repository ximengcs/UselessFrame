using System;
using System.Threading;
using System.Collections.Concurrent;

namespace UselessFrame.Net
{
    internal class NetObjectPool
    {
        private readonly ConcurrentQueue<object> _pool = new ConcurrentQueue<object>();
        private readonly int _maxSize;
        private int _count;
        private bool _disposed;

        internal int _DEBUG_requireTimes = 0;
        internal int _DEBUG_reuseTimes = 0;
        internal int _DEBUG_newTimes = 0;
        internal int _DEBUG_releaseTimes = 0;
        internal int _DEBUG_toPoolTimes = 0;
        internal int _DEBUG_wasteTimes = 0;

        public NetObjectPool(int maxSize = 128)
        {
            _maxSize = Math.Max(1, maxSize);
        }

        public object Require()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(NetObjectPool<object>));

            Interlocked.Increment(ref _DEBUG_requireTimes);

            if (_pool.TryDequeue(out var item))
            {
                Interlocked.Increment(ref _DEBUG_reuseTimes);
                Interlocked.Decrement(ref _count);
                return item;
            }

            Interlocked.Increment(ref _DEBUG_newTimes);
            return null;
        }

        public void Release(object item)
        {
            if (_disposed || item == null) return;

            Interlocked.Increment(ref _DEBUG_releaseTimes);

            if (Interlocked.Increment(ref _count) > _maxSize)
            {
                Interlocked.Increment(ref _DEBUG_wasteTimes);
                Interlocked.Decrement(ref _count);
                return;
            }

            Interlocked.Increment(ref _DEBUG_toPoolTimes);
            _pool.Enqueue(item);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            while (_pool.TryDequeue(out var item))
            {
                Interlocked.Decrement(ref _count);
            }
        }

        public int Count => _count;

        public int MaxSize => _maxSize;
    }

    internal class NetObjectPool<T> where T : class, new()
    {
        private readonly ConcurrentQueue<T> _pool = new ConcurrentQueue<T>();
        private readonly int _maxSize;
        private int _count;
        private bool _disposed;

        internal int _DEBUG_requireTimes = 0;
        internal int _DEBUG_reuseTimes = 0;
        internal int _DEBUG_newTimes = 0;
        internal int _DEBUG_releaseTimes = 0;
        internal int _DEBUG_toPoolTimes = 0;
        internal int _DEBUG_wasteTimes = 0;

        public NetObjectPool(int maxSize = 128)
        {
            _maxSize = Math.Max(1, maxSize);
        }

        public T Require()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(NetObjectPool<T>));

            Interlocked.Increment(ref _DEBUG_requireTimes);

            if (_pool.TryDequeue(out var item))
            {
                Interlocked.Increment(ref _DEBUG_reuseTimes);
                Interlocked.Decrement(ref _count);
                return item;
            }

            Interlocked.Increment(ref _DEBUG_newTimes);
            return new T();
        }

        public void Release(T item)
        {
            if (_disposed || item == null) return;

            Interlocked.Increment(ref _DEBUG_releaseTimes);
            if (Interlocked.Increment(ref _count) > _maxSize)
            {
                Interlocked.Increment(ref _DEBUG_wasteTimes);
                Interlocked.Decrement(ref _count);
                return;
            }

            Interlocked.Increment(ref _DEBUG_toPoolTimes);
            _pool.Enqueue(item);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            while (_pool.TryDequeue(out var item))
            {
                Interlocked.Decrement(ref _count);
            }
        }

        public int Count => _count;

        public int MaxSize => _maxSize;
    }
}
