
using UselessFrame.Runtime.Pools;
using XFrame.Core;

namespace XFrame.Modules.Conditions
{
    /// <summary>
    /// 条件转换器
    /// </summary>
    public class ConditionParser : IParser<ConditionData>
    {
        private ConditionData m_Value;

        /// <inheritdoc/>
        public ConditionData Value => m_Value;

        /// <inheritdoc/>
        public int PoolKey => default;

        IPool IPoolObject.InPool { get; set; }

        /// <inheritdoc/>
        public string MarkName { get; set; }

        object IParser.Value => m_Value;

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="pattern">数据</param>
        /// <returns>解析结果</returns>
        public ConditionData Parse(string pattern)
        {
            m_Value = new ConditionData(pattern);
            return m_Value;
        }

        void IPoolObject.OnCreate(object param)
        {

        }

        void IPoolObject.OnDelete()
        {

        }

        void IPoolObject.OnRelease()
        {
            m_Value = default;
        }

        void IPoolObject.OnRequest()
        {

        }

        object IParser.Parse(string pattern)
        {
            return Parse(pattern);
        }
    }
}
