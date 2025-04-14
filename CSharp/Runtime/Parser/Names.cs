using System;
using System.Collections;
using System.Collections.Generic;
using UselessFrame.Runtime.Pools;

namespace XFrame.Core
{
    /// <summary>
    /// 标识符集合
    /// <para>
    /// 例 yanying^series#add~1-10^2#2^dir#l
    /// 和<see cref="Name"/>类都存在一个主标识符，为yanying,
    /// 以上例子中包含10个标识符
    /// <para>yanying^series#1^2#2^dir#l</para>
    /// <para>yanying^series#2^2#2^dir#l</para>
    /// ...
    /// <para>yanying^series#9^2#2^dir#l</para>
    /// <para>yanying^series#10^2#2^dir#l</para>
    /// </para>
    /// </summary>
    public class Names : MapParser<IntOrHashParser, AreaParser>, IEnumerable<string>
    {
        private Comparison<IntOrHashParser> m_Compare;
        private List<IntOrHashParser> m_Keys;
        private bool m_ValueDirty;
        private List<string> m_Values;
        private HashSet<string> m_ValueMap;

        private List<string> Values
        {
            get
            {
                InnerCalculateValue();
                return m_Values;
            }
        }

        private void InnerCalculateValue()
        {
            if (m_ValueDirty)
            {
                m_Values.Clear();
                m_ValueMap.Clear();
                if (m_Compare != null)
                    m_Keys.Sort(m_Compare);
                InnerCollectNumArea(string.Empty, 0);
                m_ValueDirty = false;
            }
        }

        private bool InnerCollectNumArea(string prev, int currentIndex)
        {
            if (currentIndex >= m_Keys.Count)
                return false;
            IntOrHashParser key = m_Keys[currentIndex];
            AreaParser numArea = Get(key);
            bool isRoot = string.IsNullOrEmpty(prev);
            foreach (string str in numArea.Value)
            {
                string thisValue;
                if (key != Name.AVATAR)
                    thisValue = isRoot ? $"{key}{Split2}{str}" : $"{prev}{Split}{key}{Split2}{str}";
                else
                    thisValue = isRoot ? str : $"{prev}{Split}{str}";
                bool next = InnerCollectNumArea(thisValue, currentIndex + 1);
                if (!isRoot && !next)
                {
                    m_Values.Add(thisValue);
                    m_ValueMap.Add(thisValue);
                }
            }
            return true;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Split = Name.SPLIT3;
            Split2 = Name.SPLIT4;
            m_Keys = new List<IntOrHashParser>();
            m_Values = new List<string>();
            m_ValueMap = new HashSet<string>();
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            m_Keys.Clear();
        }

        /// <inheritdoc/>>
        protected override void InnerParseItem(out IntOrHashParser kParser, out AreaParser vParser, string[] pItem)
        {
            m_ValueDirty = true;
            if (pItem.Length == 1)
            {
                IntOrHashParser key = Name.AVATAR;
                if (Has(key))
                    throw new InputFormatException($"Name format error, multi type 0");
                kParser = key;
                vParser = _valuePool.Require();
                vParser.Parse(pItem[0]);
            }
            else
            {
                kParser = _keyPool.Require();
                vParser = _valuePool.Require();
                kParser.Parse(pItem[0]);
                vParser.Parse(pItem[1]);
            }

            m_Keys.Add(kParser);
        }

        /// <summary>
        /// 设置排序规则
        /// </summary>
        /// <param name="compareFunc"></param>
        public void SetSort(Comparison<IntOrHashParser> compareFunc)
        {
            m_Compare = compareFunc;
        }

        /// <summary>
        /// 是否存在标识名
        /// </summary>
        /// <param name="name">标识名</param>
        /// <returns>true表示存在</returns>
        public bool Has(string name)
        {
            return m_ValueMap.Contains(name);
        }

        /// <summary>
        /// 是否存在标识名
        /// </summary>
        /// <param name="name">标识名解析器</param>
        /// <returns>true表示存在</returns>
        public bool Has(Name name)
        {
            return m_ValueMap.Contains(name);
        }

        /// <summary>
        /// 检查是否相等
        /// </summary>
        /// <param name="obj">对比值</param>
        /// <returns>true表示相等</returns>
        public override bool Equals(object obj)
        {
            Names parser = obj as Names;
            if (parser == null)
            {
                string strValue = obj as string;
                if (strValue == m_Origin)
                    return true;

                if (!string.IsNullOrEmpty(strValue))
                    parser = Names.Create(strValue);

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

        private static bool InnerCompareName(Names name1, Names name2)
        {
            if (name1.Count != name2.Count)
                return false;
            foreach (var entry in name2.Value)
            {
                if (name1.TryGet(entry.Key, out AreaParser value))
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
        /// 获取哈希值
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
        /// <param name="pattern">文本</param>
        /// <returns>解析器</returns>
        public static Names Create(string pattern)
        {
            Names name = new Names();
            name.Split = Name.SPLIT3;
            name.Split2 = Name.SPLIT4;
            name.Parse(pattern);
            return name;
        }

        /// <inheritdoc/>
        public IEnumerator<string> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        /// <summary>
        /// 检查是否相等
        /// </summary>
        /// <param name="src">解析器</param>
        /// <param name="tar">对比值</param>
        /// <returns>true标识相等</returns>
        public static bool operator ==(Names src, object tar)
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
        /// 检查是否不相等
        /// </summary>
        /// <param name="src">解析器</param>
        /// <param name="tar">对比值</param>
        /// <returns>true标识不相等</returns>
        public static bool operator !=(Names src, object tar)
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
        /// 返回原始字符串
        /// </summary>
        /// <param name="parser">解析器</param>
        public static implicit operator string(Names parser)
        {
            return parser != null ? parser.m_Origin : default;
        }

        /// <summary>
        /// 将字符串转化为解析器
        /// </summary>
        /// <param name="value">字符串</param>
        public static implicit operator Names(string value)
        {
            return Create(value);
        }
    }
}
