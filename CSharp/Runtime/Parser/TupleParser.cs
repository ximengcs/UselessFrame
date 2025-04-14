using System;
using UselessFrame.Runtime.Pools;

namespace XFrame.Core
{
    /// <summary>
    /// 元组解析器
    /// </summary>
    /// <typeparam name="T1">类型1</typeparam>
    /// <typeparam name="T2">类型2</typeparam>
    public class TupleParser<T1, T2> : IParser<ValueTuple<T1, T2>> where T1 : IParser where T2 : IParser
    {
        private readonly static char SPLIT = ';';

        private IPool _pool;
        private IPool<T1> _v1Pool;
        private IPool<T2> _v2Pool;
        private ValueTuple<T1, T2> m_Value;

        /// <summary>
        /// 持有值
        /// </summary>
        public ValueTuple<T1, T2> Value => m_Value;

        object IParser.Value => m_Value;

        int IPoolObject.PoolKey => default;

        public string Name { get; set; }

        IPool IPoolObject.InPool
        {
            get => _pool;
            set => _pool = value;
        }

        /// <inheritdoc/>
        public ValueTuple<T1, T2> Parse(string pattern)
        {
            if (_v1Pool == null)
            {
                _v1Pool = (IPool<T1>)_pool.System.GetOrNew(typeof(T1));
                _v2Pool = (IPool<T2>)_pool.System.GetOrNew(typeof(T2));
            }

            pattern = pattern.Substring(1, pattern.Length - 2);
            string[] contents = pattern.Split(SPLIT);

            T1 v1 = _v1Pool.Require();
            v1.Parse(contents[0]);

            T2 v2 = _v2Pool.Require();
            v2.Parse(contents[1]);

            m_Value = new ValueTuple<T1, T2>(v1, v2);
            return m_Value;
        }

        object IParser.Parse(string pattern)
        {
            return Parse(pattern);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return m_Value.ToString();
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return m_Value.GetHashCode();
        }

        /// <summary>
        /// 释放到池中
        /// </summary>
        public void Release()
        {
            _pool.Release(this);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            IParser parser = obj as IParser;
            return parser != null ? m_Value.Equals(parser.Value) : m_Value.Equals(obj);
        }

        void IPoolObject.OnCreate()
        {
        }

        void IPoolObject.OnRequest()
        {

        }

        void IPoolObject.OnRelease()
        {

        }

        void IPoolObject.OnDelete()
        {

        }

        /// <summary>
        /// 检查两个值是否相等
        /// </summary>
        /// <param name="src">解析器</param>
        /// <param name="tar">对比值</param>
        /// <returns>true表示相等</returns>
        public static bool operator ==(TupleParser<T1, T2> src, object tar)
        {
            return src.Equals(tar);
        }

        /// <summary>
        /// 检查两个值是否不相等
        /// </summary>
        /// <param name="src">解析器</param>
        /// <param name="tar">对比值</param>
        /// <returns>true表示不相等</returns>
        public static bool operator !=(TupleParser<T1, T2> src, object tar)
        {
            return !src.Equals(tar);
        }

        /// <summary>
        /// 将值转换为解析器
        /// </summary>
        /// <param name="value">值</param>
        public static implicit operator TupleParser<T1, T2>((T1, T2) value)
        {
            TupleParser<T1, T2> parser = new TupleParser<T1, T2>();
            parser.m_Value.Item1 = value.Item1;
            parser.m_Value.Item2 = value.Item2;
            return parser;
        }

        /// <summary>
        /// 返回解析器的值
        /// </summary>
        /// <param name="value">解析器</param>
        public static implicit operator (T1, T2)(TupleParser<T1, T2> value)
        {
            return (value.m_Value.Item1, value.m_Value.Item2);
        }
    }
}