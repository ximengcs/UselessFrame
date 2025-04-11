using XFrame.Modules.Pools;
using XFrame.Modules.Diagnotics;

namespace XFrame.Core
{
    /// <summary>
    /// 标识名解析器
    /// <para>
    /// 例 yanying^series#1^layer#2^dir#l
    /// 中共有4项，其中主标识符为yanying
    /// </para>
    /// </summary>
    public class Name : MapParser<IntOrHashParser, UniversalParser>
    {
        /// <summary>
        /// 默认主标识符键
        /// </summary>
        public static int AVATAR = 0;

        /// <summary>
        /// 默认项分割符
        /// </summary>
        public static char SPLIT3 = '^';

        /// <summary>
        /// 默认键值分割符
        /// </summary>
        public static char SPLIT4 = '#';
        
        /// <inheritdoc/>
        protected override void InnerParseItem(out IntOrHashParser kParser, out UniversalParser vParser, string[] pItem)
        {
            if (pItem.Length == 1)
            {
                IntOrHashParser key = AVATAR;
                if (Has(key))
                    Log.Error(Log.XFrame, $"Name format error, multi type 0");
                kParser = key;
                vParser = References.Require<UniversalParser>();
                vParser.Parse(pItem[0]);
            }
            else
            {
                kParser = References.Require<IntOrHashParser>();
                vParser = References.Require<UniversalParser>();
                kParser.Parse(pItem[0]);
                vParser.Parse(pItem[1]);
            }
        }

        /// <summary>
        /// 判断主标识符
        /// </summary>
        /// <param name="vParser">对比值</param>
        /// <param name="keyRelease">是否自动释放键解析器</param>
        /// <param name="valueRelease">是否自动释放值解析器</param>
        /// <returns>true表示相等</returns>
        public bool Is(UniversalParser vParser, bool keyRelease = true, bool valueRelease = true)
        {
            return Is(AVATAR, vParser, keyRelease, valueRelease);
        }

        /// <summary>
        /// 判断是否存在某个标识
        /// </summary>
        /// <param name="kParser">对比标识符键</param>
        /// <param name="vParser">对比标识符值</param>
        /// <param name="keyRelease">是否自动释放键解析器</param>
        /// <param name="valueRelease">是否自动释放值解析器</param>
        /// <returns>true表示相等</returns>
        public bool Is(IntOrHashParser kParser, UniversalParser vParser, bool keyRelease = true, bool valueRelease = true)
        {
            bool result = false;
            if (TryGet(kParser, out UniversalParser target))
                result = vParser == target;
            if (keyRelease)
                kParser.Release();
            if (valueRelease)
                vParser.Release();
            return result;
        }

        /// <summary>
        /// 检查是否相等
        /// </summary>
        /// <param name="obj">对比值</param>
        /// <returns>true表示相等</returns>
        public override bool Equals(object obj)
        {
            Name parser = obj as Name;
            if (parser == null)
            {
                string strValue = obj as string;
                if (strValue == m_Origin)
                    return true;

                if (!string.IsNullOrEmpty(strValue))
                    parser = Name.Create(strValue);

                if (parser != null)
                {
                    bool equals = InnerCompareName(this, parser);
                    parser.Release();
                    return equals;
                }
            }
            else
            {
                return InnerCompareName(this, parser);
            }

            return false;
        }

        private static bool InnerCompareName(Name name1, Name name2)
        {
            if (name1.Count != name2.Count)
                return false;
            foreach (var entry in name2.Value)
            {
                if (name1.TryGet(entry.Key, out UniversalParser value))
                {
                    if (entry.Value != value)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 返回哈希值
        /// </summary>
        /// <returns>哈希值</returns>
        public override int GetHashCode()
        {
            int result = 0;
            foreach (var entry in Value)
                result += entry.GetHashCode();
            return result;
        }

        /// <summary>
        /// 创建标识符解析器
        /// </summary>
        /// <param name="pattern">待解析文本</param>
        /// <returns>标识符解析器</returns>
        public static Name Create(string pattern)
        {
            Name name = References.Require<Name>();
            name.Split = SPLIT3;
            name.Split2 = SPLIT4;
            name.Parse(pattern);
            return name;
        }

        /// <summary>
        /// 判断两个值是否相等
        /// </summary>
        /// <param name="src">标识符解析器</param>
        /// <param name="tar">对比值</param>
        /// <returns>true表示相等</returns>
        public static bool operator ==(Name src, object tar)
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
        /// 判断两个值是否不相等
        /// </summary>
        /// <param name="src">标识符解析器</param>
        /// <param name="tar">对比值</param>
        /// <returns>true表示不相等</returns>
        public static bool operator !=(Name src, object tar)
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
        /// 返回解析器原始字符串
        /// </summary>
        /// <param name="parser">解析器</param>
        public static implicit operator string(Name parser)
        {
            return parser != null ? parser.m_Origin : default;
        }

        /// <summary>
        /// 将字符串转换为解析器
        /// </summary>
        /// <param name="value">字符串</param>
        public static implicit operator Name(string value)
        {
            return Create(value);
        }
    }
}
