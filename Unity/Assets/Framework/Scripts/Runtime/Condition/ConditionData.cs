using System.Collections;
using System.Collections.Generic;
using UselessFrame.Runtime.Collections;
using XFrame.Core;
using static XFrame.Modules.Conditions.ConditionData;
using ItemParser = XFrame.Core.PairParser<XFrame.Core.IntOrHashParser, XFrame.Core.StringParser>;

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
        private Item[] _items;

        public IReadOnlyList<Item> Items => _items;

        public int Count => _items.Length;

        /// <summary>
        /// 使用原始条件构造条件配置
        /// </summary>
        /// <param name="originData">原始配置，多个项由逗号分隔，条件类型和参数用|分隔</param>
        public ConditionData(string originData)
        {
            var m_Parser = new ArrayParser<ItemParser>();
            m_Parser.Parse(originData);

            if (!m_Parser.Empty)
            {
                _items = new Item[m_Parser.Count];
                var list = m_Parser.Value;
                for (int i = 0; i < list.Count; i++)
                {
                    ItemParser parser = list[i];
                    int key = parser.Value.Key.Value;
                    string value = parser.Value.Value.Value;
                    Item item = new Item(key, value);
                    _items[i] = item;
                }
            }
            else
            {
                _items = null;
            }
        }

        /// <summary>
        /// 使用转换器构造条件配置
        /// </summary>
        /// <param name="parser"></param>
        public ConditionData(ArrayParser<ItemParser> arrParser)
        {
            if (!arrParser.Empty)
            {
                _items = new Item[arrParser.Count];
                var list = arrParser.Value;
                for (int i = 0; i < list.Count; i++)
                {
                    ItemParser parser = list[i];
                    int key = parser.Value.Key.Value;
                    string value = parser.Value.Value.Value;
                    Item item = new Item(key, value);
                    _items[i] = item;
                }
            }
            else
            {
                _items = null;
            }
        }

        /// <summary>
        /// 此配置是否包含目标类型的条件
        /// </summary>
        /// <param name="target">条件目标类型</param>
        /// <returns> true为包含，否则不包含 </returns>
        public bool Has(int target)
        {
            return Find(target).Valid;
        }

        /// <summary>
        /// 查找第一个符合目标条件类型的条件项
        /// </summary>
        /// <param name="target">条件目标类型</param>
        /// <returns>查找到的条件项</returns>
        public Item Find(int target)
        {
            foreach (Item item in _items)
            {
                if (item.Type == target)
                {
                    return item;
                }
            }
            return default;
        }

        /// <summary>
        /// 查找所有符合目标条件类型的条件项
        /// </summary>
        /// <param name="target">条件目标类型</param>
        /// <returns>查找到的条件项列表</returns>
        public List<Item> FindAll(int target)
        {
            var result = new List<Item>(Count);
            foreach (Item item in _items)
            {
                if (item.Type == target)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// 正向迭代条件项
        /// </summary>
        /// <returns>迭代器</returns>
        public IEnumerator<Item> GetEnumerator()
        {
            return GetEnumerator(EnumeratorType.Front);
        }

        public IEnumerator<Item> GetEnumerator(EnumeratorType type)
        {
            switch (type)
            {
                case EnumeratorType.Front: return new ListEnumerator<Item>(_items);
                case EnumeratorType.Back: return new ListBackEnumerator<Item>(_items);
            }
            return null;
        }

        IEnumerator IMultiEnumerable.GetEnumerator(EnumeratorType type)
        {
            return GetEnumerator(type);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
