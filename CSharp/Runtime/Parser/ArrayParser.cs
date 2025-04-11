using System;
using XFrame.Collections;
using XFrame.Modules.Pools;

namespace XFrame.Core
{
    /// <summary>
    /// 数组解析器
    /// </summary>
    /// <typeparam name="T">持有对象类型</typeparam>
    public class ArrayParser<T> : IParser<XLinkList<T>> where T : IParser
    {
        /// <summary>
        /// 默认元素分隔符
        /// </summary>
        public const char SPLIT = ',';
        private char m_Split;
        private string m_Origin;

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
        public XLinkList<T> Value { get; private set; }

        object IParser.Value => Value;

        int IPoolObject.PoolKey => default;

        /// <inheritdoc/>
        public string MarkName { get; set; }
        IPool IPoolObject.InPool { get; set; }

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
            References.Release(this);
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="pattern">文本</param>
        /// <returns>解析的元素列表</returns>
        public XLinkList<T> Parse(string pattern)
        {
            m_Origin = pattern;
            if (Value == null)
                Value = new XLinkList<T>();
            else
                Value.Clear();

            if (!string.IsNullOrEmpty(pattern))
            {
                string[] pArray = pattern.Split(m_Split);
                Type type = typeof(T);
                for (int i = 0; i < pArray.Length; i++)
                {
                    T parser = (T)References.Require(type);
                    parser.Parse(pArray[i]);
                    Value.AddLast(parser);
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
            foreach (XLinkNode<T> node in Value)
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
            foreach (XLinkNode<T> node in Value)
            {
                if (index == current)
                    return node.Value;
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
            foreach (XLinkNode<T> node in Value)
            {
                object other = node.Value.Value;
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
            foreach (XLinkNode<T> v in Value)
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
            foreach (XLinkNode<T> v in Value)
                References.Release(v.Value);
            Value.Clear();
        }

        void IPoolObject.OnDelete()
        {
            Value.Clear();
            Value = null;
        }
    }
}
