using XFrame.Modules.Pools;
using XFrame.Modules.Diagnotics;

namespace XFrame.Core
{
    /// <summary>
    /// 整形或哈希值解析器
    /// <para>
    /// 当解析整形成功时结果为整形，失败时返回字符串哈希值
    /// </para>
    /// </summary>
    public class IntOrHashParser : IntParser
    {
        private bool m_IsInt;
        private string m_Origin;

        /// <inheritdoc/>
        public override int Parse(string pattern)
        {
            m_Origin = pattern;
            if (string.IsNullOrEmpty(pattern))
            {
                m_Value = default;
                Log.Print(LogLv, Log.XFrame, $"IntParser parse failure. {pattern}");
            }
            else
            {
                if (!TryParse(pattern, out m_Value))
                {
                    m_Value = pattern.GetHashCode();
                    m_IsInt = false;
                }
                else
                {
                    m_IsInt = true;
                }
            }

            return m_Value;
        }

        /// <summary>
        /// 检查两个值是否相等
        /// </summary>
        /// <param name="obj">对比值</param>
        /// <returns>true表示相等</returns>
        public override bool Equals(object obj)
        {
            IParser parser = obj as IParser;
            object value = parser != null ? parser.Value : obj;
            if (value is int intValue)
                return m_Value.Equals(intValue);
            else if (value is string strValue)
                return m_Origin.Equals(strValue);
            else
                return false;
        }

        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns>哈希值</returns>
        public override int GetHashCode()
        {
            return m_Origin.GetHashCode();
        }

        /// <summary>
        /// 返回字符串形式
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString()
        {
            return m_IsInt ? base.ToString() : m_Origin;
        }

        /// <summary>
        /// 检查两个整形值是否相等
        /// </summary>
        /// <param name="src">解析器</param>
        /// <param name="tar">目标值</param>
        /// <returns>true表示相等</returns>
        public static bool operator ==(IntOrHashParser src, object tar)
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
        /// 检查两个整形值是否不相等
        /// </summary>
        /// <param name="src">解析器</param>
        /// <param name="tar">目标值</param>
        /// <returns>true表示不相等</returns>
        public static bool operator !=(IntOrHashParser src, object tar)
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
        /// 返回解析到的值
        /// </summary>
        /// <param name="parser">整型值</param>
        public static implicit operator int(IntOrHashParser parser)
        {
            return parser != null ? parser.m_Value : default;
        }

        /// <summary>
        /// 将整形值转换为解析器
        /// </summary>
        /// <param name="value">整形值</param>
        public static implicit operator IntOrHashParser(int value)
        {
            IntOrHashParser parser = References.Require<IntOrHashParser>();
            parser.m_Value = value;
            parser.m_Origin = value.ToString();
            return parser;
        }

        /// <summary>
        /// 返回解析器值的字符串形式
        /// </summary>
        /// <param name="parser">值的字符串形式</param>
        public static implicit operator string(IntOrHashParser parser)
        {
            return parser != null ? parser.m_Origin : default;
        }

        /// <summary>
        /// 解析字符串并返回解析器
        /// </summary>
        /// <param name="value">解析器</param>
        public static implicit operator IntOrHashParser(string value)
        {
            IntOrHashParser parser = References.Require<IntOrHashParser>();
            parser.Parse(value);
            return parser;
        }
    }
}
