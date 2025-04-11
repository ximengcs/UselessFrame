using System.Globalization;
using XFrame.Modules.Diagnotics;
using XFrame.Modules.Pools;

namespace XFrame.Core
{
    /// <summary>
    /// 浮点值解析器
    /// </summary>
    public class FloatParser : IParser<float>, ICanConfigLog
    {
        private float m_Value;

        /// <summary>
        /// 解析到的值
        /// </summary>
        public float Value => m_Value;

        /// <summary>
        /// Log等级
        /// </summary>
        public LogLevel LogLv { get; set; }

        object IParser.Value => m_Value;

        int IPoolObject.PoolKey => default;

        /// <inheritdoc/>
        public string MarkName { get; set; }
        IPool IPoolObject.InPool { get; set; }

        /// <inheritdoc/>
        public float Parse(string pattern)
        {
            if (string.IsNullOrEmpty(pattern) || !TryParse(pattern, out m_Value))
            {
                m_Value = default;
                Log.Print(LogLv, Log.XFrame, $"FloatParser parse failure. {pattern}");
            }

            return m_Value;
        }

        /// <summary>
        /// 尝试解析浮点值
        /// </summary>
        /// <param name="pattern">待解析文本</param>
        /// <param name="value">转换到的浮点值</param>
        /// <returns>true表示解析成功</returns>
        public static bool TryParse(string pattern, out float value)
        {
            return float.TryParse(pattern, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out value);
        }

        object IParser.Parse(string pattern)
        {
            return Parse(pattern);
        }

        /// <summary>
        /// 返回浮点值字符串值
        /// </summary>
        /// <returns>字符串值</returns>
        public override string ToString()
        {
            return m_Value.ToString();
        }

        /// <summary>
        /// 获取浮点值哈希值
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
            if (parser != null)
            {
                return m_Value.Equals(parser.Value);
            }
            else
            {
                if (obj is int)
                    return m_Value.Equals((int)obj);
                else
                    return m_Value.Equals((float)obj);
            }
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
            m_Value = default;
            LogLv = LogLevel.Warning;
        }

        void IPoolObject.OnRelease()
        {

        }

        void IPoolObject.OnDelete()
        {

        }

        /// <summary>
        /// 检查连个值是否相等
        /// </summary>
        /// <param name="src">浮点解析器</param>
        /// <param name="tar">对比值</param>
        /// <returns>true表示相等</returns>
        public static bool operator ==(FloatParser src, object tar)
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
        /// 检查连个值是否不相等
        /// </summary>
        /// <param name="src">浮点解析器</param>
        /// <param name="tar">对比值</param>
        /// <returns>true表示不相等</returns>
        public static bool operator !=(FloatParser src, object tar)
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
        /// 返回解析器的浮点值
        /// </summary>
        /// <param name="parser">浮点值</param>
        public static implicit operator float(FloatParser parser)
        {
            return parser != null ? parser.m_Value : default;
        }

        /// <summary>
        /// 将浮点值转换为解析器
        /// </summary>
        /// <param name="value">浮点值</param>
        public static implicit operator FloatParser(float value)
        {
            FloatParser parser = References.Require<FloatParser>();
            parser.m_Value = value;
            return parser;
        }
    }
}
