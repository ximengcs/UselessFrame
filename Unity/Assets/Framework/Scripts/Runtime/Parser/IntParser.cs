using System.Globalization;
using UselessFrame.Runtime.Pools;

namespace XFrame.Core
{
    /// <summary>
    /// 整数解析器
    /// </summary>
    public class IntParser : IParser<int>
    {
        private IPool _pool;

        /// <summary>
        /// 持有值
        /// </summary>
        protected int m_Value;

        /// <summary>
        /// 持有值
        /// </summary>
        public int Value => m_Value;

        object IParser.Value => m_Value;

        int IPoolObject.PoolKey => default;

        /// <inheritdoc/>
        public string Name { get; set; }

        IPool IPoolObject.InPool
        {
            get => _pool;
            set => _pool = value;
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="pattern">待解析文本</param>
        /// <returns>解析到的值</returns>
        public virtual int Parse(string pattern)
        {
            if (string.IsNullOrEmpty(pattern) || !TryParse(pattern, out m_Value))
            {
                m_Value = default;
                throw new InputFormatException($"IntParser parse failure. {pattern}");
            }

            return m_Value;
        }

        /// <summary>
        /// 尝试解析
        /// </summary>
        /// <param name="pattern">待解析的值</param>
        /// <param name="value">文本</param>
        /// <returns>是否解析成功</returns>
        public static bool TryParse(string pattern, out int value)
        {
            return int.TryParse(pattern, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }

        object IParser.Parse(string pattern)
        {
            return Parse(pattern);
        }

        /// <summary>
        /// 值字符串形式
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString()
        {
            return m_Value.ToString();
        }

        /// <summary>
        /// 返回值哈希码
        /// </summary>
        /// <returns>哈希码</returns>
        public override int GetHashCode()
        {
            return m_Value.GetHashCode();
        }

        /// <summary>
        /// 检查是否相等
        /// </summary>
        /// <param name="obj">对比值</param>
        /// <returns>true为相等</returns>
        public override bool Equals(object obj)
        {
            IParser parser = obj as IParser;
            return parser != null ? m_Value.Equals(parser.Value) : m_Value.Equals(obj);
        }

        /// <summary>
        /// 释放到池中
        /// </summary>
        public void Release()
        {
            _pool.Release(this);
        }

        void IPoolObject.OnCreate(object param)
        {

        }

        void IPoolObject.OnRequest()
        {
            m_Value = default;
        }

        void IPoolObject.OnRelease()
        {

        }

        void IPoolObject.OnDelete()
        {

        }

        /// <summary>
        /// 检查是否相等
        /// </summary>
        /// <param name="src">解析器</param>
        /// <param name="tar">对比值</param>
        /// <returns>true表示相等</returns>
        public static bool operator ==(IntParser src, object tar)
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
        /// <param name="tar">对比值</param>
        /// <returns>true表示不相等</returns>
        public static bool operator !=(IntParser src, object tar)
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
        /// 返回解析器值
        /// </summary>
        /// <param name="parser">解析器</param>
        public static implicit operator int(IntParser parser)
        {
            return parser != null ? parser.m_Value : default;
        }

        /// <summary>
        /// 将值转换为解析器
        /// </summary>
        /// <param name="value">待转换的值</param>
        public static implicit operator IntParser(int value)
        {
            IntParser parser = new IntParser();
            parser.m_Value = value;
            return parser;
        }
    }
}
