using XFrame.Core;
using XFrame.Modules.Event;

namespace XFrame.Modules.Conditions
{
    /// <summary>
    /// 条件监听模块 
    /// </summary>
    public interface IConditionModule : IModule
    {
        /// <summary>
        /// 当需要触发某个条件时 
        /// 触发 <see cref="ConditionEvent"/> 
        ///     <see cref="ConditionGroupEvent"/> 
        ///     <see cref="SpecificConditionEvent"/> 
        /// 事件到此事件系统
        /// </summary>
        IEventSystem Event { get; }

        /// <summary>
        /// 获取条件组句柄
        /// </summary>
        /// <param name="name">组名称</param>
        /// <returns>条件组句柄</returns>
        IConditionGroupHandle Get(string name);

        /// <summary>
        /// 注册条件实例 查看<see cref="ConditionSetting"/>具体参数
        /// </summary>
        /// <param name="setting">条件配置</param>
        /// <returns>条件组句柄</returns>
        IConditionGroupHandle Register(ConditionSetting setting);

        /// <summary>
        /// 取消条件注册
        /// </summary>
        /// <param name="name">条件名</param>
        void UnRegister(string name);

        /// <summary>
        /// 取消条件注册
        /// </summary>
        /// <param name="handle">条件组句柄</param>
        void UnRegister(IConditionGroupHandle handle);
    }
}
