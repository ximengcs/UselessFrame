using System;
using System.Collections.Generic;
using UselessFrame.Runtime.Pools;

namespace XFrame.Core
{
    /// <summary>
    /// 数组解析器
    /// </summary>
    /// <typeparam name="T">持有对象类型</typeparam>
    public class ArrayParser<T> : IParser<List<T>> where T : IParser, new()
    {
        /// <summary>
        /// 默认元素分隔符
        /// </summary>
        public const char SPLIT = ',';
        private char m_Split;
        private string m_Origin;
        private IPool _pool;
        private IPool<T> _childPool;

        /// <summary>
        /// 元素数量
        /// </summary>
        public int Count => Value != null ? Value.Count : 0;

        /// <summary>
        /// 是否为空
        /// </summary>
        public bool Empty => Value != null ? Value.Count == 0 : true;

        /// <summary>
        /// 获取元素列表
        /// </summary>
        public List<T> Value { get; private set; }

        object IParser.Value => Value;

        int IPoolObject.PoolKey => default;

        /// <inheritdoc/>
        public string Name { get; set; }

        IPool IPoolObject.InPool
        {
            get => _pool;
            set => _pool = value;
        }

        /// <summary>
        /// 分割符
        /// </summary>
        public char Split
        {
            get => m_Split;
            set
            {
                if (m_Split != value)
                {
                    m_Split = value;
                    Parse(m_Origin);
                }
            }
        }

        /// <summary>
        /// 构造器
        /// </summary>
        public ArrayParser()
        {
            m_Split = SPLIT;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="splitchar">分隔符</param>
        public ArrayParser(char splitchar)
        {
            m_Split = splitchar;
        }

        /// <summary>
        /// 释放到池中
        /// </summary>
        public void Release()
        {
            if (_pool != null)
                _pool.Release(this);
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="pattern">文本</param>
        /// <returns>解析的元素列表</returns>
        public List<T> Parse(string pattern)
        {
            m_Origin = pattern;
            if (Value == null)
                Value = new List<T>();
            else
                Value.Clear();

            if (_pool != null && _childPool == null)
                _childPool = _pool.System.GetOrNew<T>();

            if (!string.IsNullOrEmpty(pattern))
            {
                string[] pArray = pattern.Split(m_Split);
                Type type = typeof(T);
                for (int i = 0; i < pArray.Length; i++)
                {
                    T parser;
                    if (_childPool != null)
                        parser = _childPool.Require();
                    else
                        parser = new T();
                    parser.Parse(pArray[i]);
                    Value.Add(parser);
                }
            }

            return Value;
        }

        /// <summary>
        /// 获取值的下标
        /// </summary>
        /// <param name="value">待检查的值</param>
        /// <returns>下标</returns>
        public int IndexOf(object value)
        {
            if (Value == null)
                return -1;
            int index = 0;
            foreach (T node in Value)
            {
                object other = node.Value;
                if (other != null && other.Equals(value))
                    return index;
                index++;
            }
            return -1;
        }

        /// <summary>
        /// 判断是否存在值
        /// </summary>
        /// <param name="value">待检查的值</param>
        /// <returns>true表示存在</returns>
        public bool Has(object value)
        {
            return IndexOf(value) != -1;
        }

        /// <summary>
        /// 通过下标获取值
        /// </summary>
        /// <param name="index">下标</param>
        /// <returns>值</returns>
        public T Get(int index)
        {
            if (Value == null)
                return default;
            int current = 0;
            foreach (T node in Value)
            {
                if (index == current)
                    return (T)node.Value;
                current++;
            }
            return default;
        }

        /// <summary>
        /// 获取值下标
        /// </summary>
        /// <param name="value">待检查的值</param>
        /// <param name="action">判断函数</param>
        /// <returns>下标</returns>
        public int IndexOf(object value, Func<object, object, bool> action)
        {
            if (Value == null)
                return -1;
            int index = 0;
            foreach (T node in Value)
            {
                object other = node.Value;
                if (other != null && action(value, other))
                    return index;
                index++;
            }
            return -1;
        }

        /// <summary>
        /// 是否包含某个值
        /// </summary>
        /// <param name="value">待检查的值</param>
        /// <param name="action">判断函数</param>
        /// <returns>true为包含</returns>
        public bool Has(object value, Func<object, object, bool> action)
        {
            return IndexOf(value, action) != -1;
        }

        object IParser.Parse(string pattern)
        {
            return Parse(pattern);
        }

        /// <summary>
        /// 原始值
        /// </summary>
        /// <returns>原始值</returns>
        public override string ToString()
        {
            return m_Origin;
        }

        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns>哈希值</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 判断两个值是否相等
        /// </summary>
        /// <param name="obj">对比值</param>
        /// <returns>true表示相等</returns>
        public override bool Equals(object obj)
        {
            if (Value == null)
            {
                if (obj != null)
                    return false;
                else
                    return true;
            }
            foreach (T v in Value)
            {
                if (!v.Equals(obj))
                    return false;
            }
            return true;
        }

        void IPoolObject.OnCreate()
        {

        }

        void IPoolObject.OnRequest()
        {
            m_Split = SPLIT;
        }

        void IPoolObject.OnRelease()
        {
            if (_childPool != null)
            {
                foreach (T v in Value)
                    _childPool.Release(v);
            }
            
            Value.Clear();
        }

        void IPoolObject.OnDelete()
        {
            Value.Clear();
            Value = null;
        }
    }
}
