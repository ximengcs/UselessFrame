
using System;
using System.Collections;
using System.Collections.Generic;
using UselessFrame.Runtime.Pools;

namespace UselessFrame.Runtime.Collections
{
    public class XList<T> : IList<T>, IPoolObject
    {
        private System.Collections.Generic.List<T> _list;
        private Action _onChange;

        public event Action OnChangeEvent
        {
            add => _onChange += value;
            remove => _onChange -= value;
        }

        public T this[int index]
        {
            get => _list[index];
            set
            {
                _list[index] = value;
                _onChange?.Invoke();
            }
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public int PoolKey => _list.Capacity;

        public string Name { get; set; }

        IPool IPoolObject.InPool { get; set; }

        public XList()
        {
            _list = new System.Collections.Generic.List<T>();
        }

        public XList(int capacity)
        {
            _list = new System.Collections.Generic.List<T>(capacity);
        }

        public void Add(T item)
        {
            _list.Add(item);
            _onChange?.Invoke();
        }

        public void Clear()
        {
            _list.Clear();
            _onChange?.Invoke();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
            _onChange?.Invoke();
        }

        public bool Remove(T item)
        {
            bool success = _list.Remove(item);
            _onChange?.Invoke();
            return success;
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
            _onChange?.Invoke();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void IPoolObject.OnCreate()
        {

        }

        void IPoolObject.OnDelete()
        {
            _list = null;
        }

        void IPoolObject.OnRelease()
        {
            _list.Clear();
        }

        void IPoolObject.OnRequest()
        {

        }
    }
}
