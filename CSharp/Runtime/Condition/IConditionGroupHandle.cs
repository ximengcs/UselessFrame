using System;
using System.Collections.Generic;
using UselessFrame.Runtime.Collections;

namespace XFrame.Modules.Conditions
{
    /// <summary>
    /// 条件组句柄
    /// <para>
    /// 条件组内可有多个条件项，多个条件类型可以相同。
    /// </para>
    /// </summary>
    public interface IConditionGroupHandle
    {
        /// <summary>
        /// 条件辅助器<see cref="IConditionHelper"/>的实例Id，默认使用全局辅助器
        /// </summary>
        int InstanceId { get; }

        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 条件是否完成
        /// </summary>
        bool Complete { get; }

        /// <summary>
        /// 条件配置
        /// </summary>
        ConditionSetting Setting { get; }

        /// <summary>
        /// 组内所有的条件项句柄
        /// </summary>
        List<IConditionHandle> AllInfo { get; }

        /// <summary>
        /// 组内还未达成的条件项句柄
        /// </summary>
        Dictionary<int, List<IConditionHandle>> NotInfo { get; }

        IDataProvider Data { get; }

        /// <summary>
        /// 注册条件组完成事件回调
        /// </summary>
        /// <param name="callback">回调</param>
        void OnComplete(Action<IConditionGroupHandle> callback);
    }
}
