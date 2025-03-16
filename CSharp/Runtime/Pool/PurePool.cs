using System.Collections.Generic;

namespace UselessFrame.Runtime.Pools
{
    internal class PurePool<T>
    {
        private Queue<T> _queue;

        public PurePool(int capacity = 64)
        {
            _queue = new Queue<T>(capacity);
        }

        public T Require()
        {
            if (_queue.Count == 0)
                return default(T);

            return _queue.Dequeue();
        }

        public void Release(T obj)
        {
            _queue.Enqueue(obj);
        }
    }
}
