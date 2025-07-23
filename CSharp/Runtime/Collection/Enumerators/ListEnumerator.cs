
using System.Collections;
using System.Collections.Generic;
using UselessFrame.Runtime.Pools;

namespace UselessFrame.Runtime.Collections
{
    public struct ListEnumerator<T> : IEnumerator<T>
    {
        private XList<T> _list;
        private int _index;
        private int _count;
        private IPool<XList<T>> _pool;

        public T Current => _list[_index];

        object IEnumerator.Current => _list[_index];

        public ListEnumerator(IList<T> target, bool clone = true, IPool<XList<T>> pool = null)
        {
            _pool = pool;
            _list = pool.Require();
            if (_list == null)
                _list = new XList<T>();

            foreach (T item in _list)
                _list.Add(item);
            _count = _list.Count;
            _index = -1;
        }

        public void Dispose()
        {
            _list.Clear();
            if (_pool != null)
            {
                _pool.Release(_list);
                _pool = null;
            }
            _list = null;
        }

        public bool MoveNext()
        {
            return ++_index < _count;
        }

        public void Reset()
        {
            _index = -1;
        }
    }
}
