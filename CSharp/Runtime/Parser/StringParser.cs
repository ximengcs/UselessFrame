
using XFrame.Modules.Pools;

namespace XFrame.Core
{
    /// <summary>
    /// 字符串解析器
    /// </summary>
    public class StringParser : IParser<string>
    {
        /// <summary>
        /// 持有值
        /// </summary>
        public string Value { get; private set; }

        object IParser.Value => Value;

        int IPoolObject.PoolKey => default;
        /// <inheritdoc/>
        public string MarkName { get; set; }
        IPool IPoolObject.InPool { get; set; }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="pattern">文本</param>
        /// <returns>文本</returns>
        public string Parse(string pattern)
        {
            Value = string.IsNullOrEmpty(pattern) ? string.Empty : pattern;
            return Value;
        }

        object IParser.Parse(string pattern)
        {
            return Parse(pattern);
        }

        /// 返回字符串
        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        /// 字符串哈希值
        /// </summary>
        /// <returns>哈希值</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// 检查两个值是否相等
        /// </summary>
        /// <param name="obj">对比值</param>
        /// <returns>true表示相等</returns>
        public override bool Equals(object obj)
        {
            IParser parser = obj as IParser;
            return parser != null ? Value.Equals(parser.Value) : Value.Equals(obj);
        }

        /// <summary>
        /// 释放到池中
        /// </summary>
        public void Release()
        {
            References.Release(this);
        }

        void IPoolObject.OnCreate()
        {

        }

        void IPoolObject.OnRequest()
        {
            Value = null;
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
        public static bool operator ==(StringParser src, object tar)
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
        public static bool operator !=(StringParser src, object tar)
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
        /// 返回解析器的值
        /// </summary>
        /// <param name="parser">解析器</param>
        public static implicit operator string(StringParser parser)
        {
            return parser != null ? parser.Value : default;
        }

        /// <summary>
        /// 将字符串转换为解析器
        /// </summary>
        /// <param name="value">字符串</param>
        public static implicit operator StringParser(string value)
        {
            StringParser parser = References.Require<StringParser>();
            parser.Value = string.IsNullOrEmpty(value) ? string.Empty : value;
            return parser;
        }
    }
}
