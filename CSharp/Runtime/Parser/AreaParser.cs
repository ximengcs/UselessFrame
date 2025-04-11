using XFrame.Modules.Pools;
using System.Collections.Generic;
using XFrame.Modules.Diagnotics;

namespace XFrame.Core
{
    /// <summary>
    /// 数域解析器
    /// <para>
    /// 例:包含1-10(剔除3)，190-192，可写作 add#1-10@remove#3@add#190-192
    /// </para>
    /// </summary>
    public partial class AreaParser : PoolObjectBase, IParser<IEnumerable<string>>
    {
        /// <summary>
        /// 通用单项分割符
        /// </summary>
        public const char SPLIT = '@';

        /// <summary>
        /// 通用单项键值分割符
        /// </summary>
        public const char SPLIT2 = '#';

        /// <summary>
        /// 通用值分割符，分割开最小值最大值
        /// </summary>
        public const char SPLIT3 = '-';

        /// <summary>
        /// 通用添加关键符
        /// </summary>
        public const string KEY1 = "add";

        /// <summary>
        /// 通用移除关键符
        /// </summary>
        public const string KEY2 = "remove";

        private string m_RawValue;
        private bool m_ValueDirty;
        private List<string> m_Values;
        private HashSet<string> m_MapValues;
        private List<Info> m_AddInfos;
        private List<Info> m_RemoveInfos;

        /// <summary>
        /// 单项分割符
        /// </summary>
        public char Split { get; set; }

        /// <summary>
        /// 单项键值分割符
        /// </summary>
        public char Split2 { get; set; }

        /// <summary>
        /// 值分割符，分割开最小值最大值
        /// </summary>
        public char Split3 { get; set; }

        object IParser.Value => Value;

        /// <summary>
        /// 获取域内所有值
        /// </summary>
        public IEnumerable<string> Value
        {
            get
            {
                InnerCalculateValue();
                return m_Values;
            }
        }

        /// <summary>
        /// 检查单项文本项是否有效
        /// </summary>
        /// <param name="content">文本项</param>
        /// <returns>true为有效</returns>
        public static bool CheckValidHead(string content)
        {
            return content.StartsWith(KEY1) || content.StartsWith(KEY2);
        }

        /// <inheritdoc/>
        protected internal override void OnCreateFromPool()
        {
            base.OnCreateFromPool();
            m_MapValues = new HashSet<string>();
            m_Values = new List<string>();
            m_AddInfos = new List<Info>();
            m_RemoveInfos = new List<Info>();
        }

        /// <inheritdoc/>
        protected internal override void OnRequestFromPool()
        {
            base.OnRequestFromPool();
            Split = SPLIT;
            Split2 = SPLIT2;
            Split3 = SPLIT3;
        }

        /// <inheritdoc/>
        protected internal override void OnReleaseFromPool()
        {
            base.OnReleaseFromPool();
            m_Values.Clear();
            m_MapValues.Clear();
            m_AddInfos.Clear();
            m_RemoveInfos.Clear();
        }

        /// <inheritdoc/>
        public IEnumerable<string> Parse(string pattern)
        {
            m_RawValue = pattern;
            m_ValueDirty = true;
            if (!string.IsNullOrEmpty(pattern))
            {
                string[] items = pattern.Split(Split);
                foreach (string item in items)
                {
                    string[] kv = item.Split(Split2);
                    if (kv.Length == 2)
                    {
                        Info info = new Info(kv[1], Split3);
                        string k = kv[0];
                        if (k == KEY1)
                        {
                            m_AddInfos.Add(info);
                        }
                        else if (k == KEY2)
                        {
                            m_RemoveInfos.Add(info);
                        }
                        else
                        {
                            Log.Error(Log.XFrame, $"NumAreaParse Error {pattern}");
                        }
                    }
                    else
                    {
                        Info info = new Info(kv[0], Split3);
                        m_AddInfos.Add(info);
                    }
                }
            }
            return Value;
        }

        object IParser.Parse(string pattern)
        {
            return Parse(pattern);
        }

        private void InnerCalculateValue()
        {
            if (m_ValueDirty)
            {
                m_MapValues.Clear();
                m_Values.Clear();
                HashSet<string> addTmp = new HashSet<string>();
                HashSet<string> removeTmp = new HashSet<string>();

                foreach (Info info in m_AddInfos)
                    info.Calcalute(addTmp);
                foreach (Info info in m_RemoveInfos)
                    info.Calcalute(removeTmp);

                foreach (string str in addTmp)
                {
                    if (!removeTmp.Contains(str))
                    {
                        m_Values.Add(str);
                        m_MapValues.Add(str);
                    }
                }

                m_ValueDirty = false;
            }
        }

        /// <summary>
        /// 是否存在某个值
        /// </summary>
        /// <param name="str">对比项</param>
        /// <returns>true表示存在</returns>
        public bool Has(string str)
        {
            return m_MapValues.Contains(str);
        }

        /// <summary>
        /// 原始字符串
        /// </summary>
        /// <returns>原始串</returns>
        public override string ToString()
        {
            return m_RawValue;
        }

        /// <summary>
        /// 原始字符串哈希值
        /// </summary>
        /// <returns>哈希值</returns>
        public override int GetHashCode()
        {
            return m_RawValue.GetHashCode();
        }

        /// <summary>
        /// 释放到池中
        /// </summary>
        public void Release()
        {
            References.Release(this);
        }

        /// <summary>
        /// 判断相等性
        /// </summary>
        /// <param name="obj">对比项</param>
        /// <returns>true表示相等</returns>
        public override bool Equals(object obj)
        {
            AreaParser parser = obj as AreaParser;
            if (parser != null)
            {
                return m_RawValue.Equals(parser.m_RawValue);
            }
            else
            {
                string strValue = obj as string;
                return strValue == m_RawValue;
            }
        }

        /// <summary>
        /// 判断相等性
        /// </summary>
        /// <param name="src">域对象</param>
        /// <param name="tar">目标对象</param>
        /// <returns>true表示相等</returns>
        public static bool operator ==(AreaParser src, object tar)
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
        /// 判断不等性
        /// </summary>
        /// <param name="src">域对象</param>
        /// <param name="tar">目标对象</param>
        /// <returns>true表示不相等</returns>
        public static bool operator !=(AreaParser src, object tar)
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
        /// 输出原始字符串
        /// </summary>
        /// <param name="parser">域对象</param>
        public static implicit operator string(AreaParser parser)
        {
            return parser != null ? parser.m_RawValue : default;
        }

        /// <summary>
        /// 将原始字符串转为域
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator AreaParser(string value)
        {
            AreaParser parser = References.Require<AreaParser>();
            parser.Parse(value);
            return parser;
        }
    }
}
