using UselessFrame.Runtime.Pools;

namespace XFrame.Modules.Conditions
{
    /// <summary>
    /// 条件(组)辅助器
    /// <para>
    /// 在条件组初始化时首先会调用<see cref="CheckFinish"/>检查条件组完成状态，若为完成则直接进入完成状态。若未完成则会开始监听条件的完成，
    /// 在达成完成条件时会调用<see cref="MarkFinish"/>标记未完成状态
    /// </para>
    /// </summary>
    public interface IConditionHelper : IPoolObject
    {
        /// <summary>
        /// 辅助器类型 
        /// <para>
        /// 注意不同于 <see cref="ConditionEvent.Target"/>，<see cref="ConditionHandle.Target"/>, <see cref="IConditionCompare.Target"/>
        /// </para>
        /// </summary>
        int Type { get; }

        /// <summary>
        /// 标记条件组完成
        /// </summary>
        /// <param name="group">条件组</param>
        void MarkFinish(IConditionGroupHandle group);

        /// <summary>
        /// 检查条件组是否完成
        /// </summary>
        /// <param name="group">条件组</param>
        /// <returns>true为完成，反之亦然</returns>
        bool CheckFinish(IConditionGroupHandle group);
    }
}
