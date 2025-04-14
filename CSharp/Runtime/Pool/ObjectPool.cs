using System;
using System.Collections.Generic;

namespace UselessFrame.Runtime.Pools
{
    internal class ObjectPool<T> : IPool<T> where T : IPoolObject
    {
        private Type m_Type;
        private IPoolHelper m_Helper;
        private IPoolSystem m_Module;
        private LinkedList<T> m_Objects;
        private Queue<LinkedListNode<T>> m_NodeCache;
        private int m_UseCount;

        public Type ObjectType => m_Type;

        public int ObjectCount => m_Objects.Count;

        public int UseCount => m_UseCount;

        public IPoolHelper Helper => m_Helper;

        public IPoolSystem System => m_Module;

        public ObjectPool(IPoolSystem module, IPoolHelper helper)
        {
            m_UseCount = 0;
            m_Type = typeof(T);
            m_Helper = helper;
            m_Module = module;
            m_Objects = new LinkedList<T>();
        }

        public T Require(int poolKey, object userData = default)
        {
            T obj = (T)InnerRequire(poolKey, userData);
            m_UseCount++;
            return obj;
        }

        IPoolObject IPool.Require(int poolKey, object userData)
        {
            IPoolObject obj = InnerRequire(poolKey, userData);
            m_UseCount++;
            return obj;
        }

        public void Release(T obj)
        {
            if (InnerRelease(obj, true))
                m_UseCount--;
        }

        public void Release(IPoolObject obj)
        {
            if (InnerRelease(obj, true))
                m_UseCount--;
        }

        public void Spawn(int poolKey, int count, object userData = default, List<IPoolObject> toList = null)
        {
            for (int i = 0; i < count; i++)
            {
                IPoolObject obj = InnerCreate(poolKey, userData);
                InnerRelease(obj, false);
                if (toList != null)
                    toList.Add(obj);
            }
        }

        private IPoolObject InnerCreate(int poolKey, object userData)
        {
            IPoolObject obj = m_Helper.Factory(m_Type, poolKey, userData);
            obj.InPool = this;
            m_Helper.OnObjectCreate(obj);
            obj.OnCreate();
            return obj;
        }

        private IPoolObject InnerRequire(int poolKey, object userData)
        {
            IPoolObject obj;
            if (m_Objects.Count == 0)
            {
                obj = InnerCreate(poolKey, userData);
            }
            else
            {
                LinkedListNode<T> node = m_Objects.First;
                while (node != null)
                {
                    if (node.Value.PoolKey == poolKey)
                        break;
                    node = node.Next;
                }

                if (node != null)
                {
                    obj = node.Value;
                    obj.InPool = this;
                    m_Objects.Remove(node);
                    m_NodeCache.Enqueue(node);
                }
                else
                {
                    obj = InnerCreate(poolKey, userData);
                }
            }

            m_Helper.OnObjectRequest(obj);
            obj.OnRequest();
            return obj;
        }

        private bool InnerRelease(IPoolObject obj, bool check)
        {
            if (check && obj.InPool == null)
            {
                return false;
            }

            m_Helper.OnObjectRelease(obj);
            obj.OnRelease();
            obj.InPool = null;
            if (m_NodeCache.Count == 0)
            {
                m_Objects.AddLast((T)obj);
            }
            else
            {
                LinkedListNode<T> node = m_NodeCache.Dequeue();
                node.Value = (T)obj;
                m_Objects.AddLast(node);
            }
            return true;
        }

        public void ClearObject()
        {
            foreach (T obj in m_Objects)
            {
                m_Helper.OnObjectDestroy(obj);
                obj.OnDelete();
            }
            m_Objects.Clear();
            m_UseCount = 0;
        }
    }
}
