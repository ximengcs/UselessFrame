
using UselessFrame.Runtime.Pools;

namespace XFrame.Core
{
    /// <summary>
    /// 布尔值解析器
    /// </summary>
    public class BoolParser : IParser<bool>
    {
        private bool m_Value;
        private IPool _pool;

        /// <summary>
        /// 值
        /// </summary>
        public bool Value => m_Value;

        object IParser.Value => m_Value;

        int IPoolObject.PoolKey => default;

        /// <inheritdoc/>
        public string Name { get; set; }

        IPool IPoolObject.InPool
        {
            get => _pool;
            set => _pool = value;
        }

        /// <inheritdoc/>
        public bool Parse(string pattern)
        {
            if (string.IsNullOrEmpty(pattern) || !TryParse(pattern, out m_Value))
            {
                m_Value = default;
                throw new InputFormatException($"BoolParser parse failure. {pattern}");
            }

            return m_Value;
        }

        /// <summary>
        /// 尝试解析布尔值
        /// </summary>
        /// <param name="pattern">文本值</param>
        /// <param name="value">布尔值</param>
        /// <returns>是否成功</returns>
        public static bool TryParse(string pattern, out bool value)
        {
            if (!bool.TryParse(pattern, out value))
            {
                if (IntParser.TryParse(pattern, out int intResult))
                    return intResult != 0 ? true : false;
                else
                    return false;
            }
            else
            {
                return true;
            }
        }

        object IParser.Parse(string pattern)
        {
            return Parse(pattern);
        }

        /// <summary>
        /// 释放到池中
        /// </summary>
        public void Release()
        {
            _pool.Release(this);
        }

        /// <summary>
        /// 原始值
        /// </summary>
        /// <returns>原始值</returns>
        public override string ToString()
        {
            return m_Value.ToString();
        }

        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns>哈希值</returns>
        public override int GetHashCode()
        {
            return m_Value.GetHashCode();
        }

        /// <summary>
        /// 检查两个值是否相等
        /// </summary>
        /// <param name="obj">待检查的值</param>
        /// <returns>true表示相等</returns>
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
            m_Value = default;
        }

        void IPoolObject.OnDelete()
        {

        }

        /// <summary>
        /// 检查两个值是否相等
        /// </summary>
        /// <param name="src">解析器</param>
        /// <param name="tar">待比较的值</param>
        /// <returns>true表示相等</returns>
        public static bool operator ==(BoolParser src, object tar)
        {
            if (ReferenceEquals(src, null))
            {
                return ReferenceEquals(tar, null);
            }
            else
            {
                return src.Equals(tar);
            }
        }

        /// <summary>
        /// 检查两个值是否不相等
        /// </summary>
        /// <param name="src">解析器</param>
        /// <param name="tar">待比较的值</param>
        /// <returns>true表示不相等</returns>
        public static bool operator !=(BoolParser src, object tar)
        {
            if (ReferenceEquals(src, null))
            {
                return !ReferenceEquals(tar, null);
            }
            else
            {
                return !src.Equals(tar);
            }
        }

        /// <summary>
        /// 返回解析器的布尔值
        /// </summary>
        /// <param name="parser">解析器</param>
        public static implicit operator bool(BoolParser parser)
        {
            return parser != null ? parser.m_Value : default;
        }

        /// <summary>
        /// 将布尔值转换为解析器
        /// </summary>
        /// <param name="value">布尔值</param>
        public static implicit operator BoolParser(bool value)
        {
            BoolParser parser = new BoolParser();
            parser.m_Value = value;
            return parser;
        }
    }
}
