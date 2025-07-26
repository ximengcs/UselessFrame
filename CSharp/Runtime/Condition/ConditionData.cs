using System.Collections.Generic;
using UselessFrame.Runtime.Collections;
using XFrame.Core;
using static XFrame.Modules.Conditions.ConditionData;
using ItemParser = XFrame.Core.PairParser<XFrame.Core.IntOrHashParser, XFrame.Core.UniversalParser>;

namespace XFrame.Modules.Conditions
{
    /// <summary>
    /// 条件配置数据(可有多个条件项)
    /// <para>
    /// 默认多个条件项用逗号分隔，条件的类型和参数用|分隔，参数为一个<see cref="UniversalParser"/>,
    /// 可调用<see cref="UniversalParser.AddParser"/>自定义数值转换器
    /// </para>
    /// </summary>
    public partial struct ConditionData : IMultiEnumerable<Item>
    {
        private ItemParser m_First;
        private ItemParser m_Last;
        private ArrayParser<ItemParser> m_Parser;

        /// <summary>
        /// 条件项列表
        /// </summary>
        public ArrayParser<ItemParser> Parser => m_Parser;

        /// <summary>
        /// 第一个条件项
        /// </summary>
        public ItemParser First => m_First;

        /// <summary>
        /// 最后一个条件项
        /// </summary>
        public ItemParser Last => m_Last;

        /// <summary>
        /// 使用原始条件构造条件配置
        /// </summary>
        /// <param name="originData">原始配置，多个项由逗号分隔，条件类型和参数用|分隔</param>
        public ConditionData(string originData)
        {
            m_Parser = new ArrayParser<ItemParser>();
            m_Parser.Parse(originData);

            if (!m_Parser.Empty)
            {
                m_First = m_Parser.Value[0].Value;
                m_Last = m_Parser.Value.Last.Value;
            }
            else
            {
                m_First = null;
                m_Last = null;
            }
        }

        /// <summary>
        /// 使用转换器构造条件配置
        /// </summary>
        /// <param name="parser"></param>
        public ConditionData(ArrayParser<ItemParser> parser)
        {
            m_Parser = parser;
            if (!m_Parser.Empty)
            {
                m_First = m_Parser.Value.First.Value;
                m_Last = m_Parser.Value.Last.Value;
            }
            else
            {
                m_First = null;
                m_Last = null;
            }
        }

        /// <summary>
        /// 此配置是否包含目标类型的条件
        /// </summary>
        /// <param name="target">条件目标类型</param>
        /// <returns> true为包含，否则不包含 </returns>
        public bool Has(int target)
        {
            return Find(target) != null;
        }

        /// <summary>
        /// 查找第一个符合目标条件类型的条件项
        /// </summary>
        /// <param name="target">条件目标类型</param>
        /// <returns>查找到的条件项</returns>
        public ItemParser Find(int target)
        {
            foreach (var itemNode in Parser.Value)
            {
                if (itemNode.Value.Value.Key == target)
                {
                    return itemNode.Value.Value;
                }
            }
            return default;
        }

        /// <summary>
        /// 查找所有符合目标条件类型的条件项
        /// </summary>
        /// <param name="target">条件目标类型</param>
        /// <returns>查找到的条件项列表</returns>
        public List<ItemParser> FindAll(int target)
        {
            var result = new List<ItemParser>(Parser.Value.Count);
            foreach (var itemNode in Parser.Value)
            {
                if (itemNode.Value.Value.Key == target)
                {
                    result.Add(itemNode.Value);
                }
            }
            return result;
        }

        /// <summary>
        /// 输出条件列表
        /// </summary>
        /// <returns>条件项列表</returns>
        public override string ToString()
        {
            return Parser.ToString();
        }

        /// <summary>
        /// 正向迭代条件项
        /// </summary>
        /// <returns>迭代器</returns>
        public IEnumerator<ItemParser> GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// 迭代类型设置未支持
        /// </summary>
        public void SetIt(XItType type)
        {

        }
    }
}
