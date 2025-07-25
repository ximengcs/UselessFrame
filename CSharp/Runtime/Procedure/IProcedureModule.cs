
using System;
using XFrame.Core;
using XFrame.Modules.StateMachine;

namespace XFrame.Modules.Procedure
{
    /// <summary>
    /// 流程模块
    /// </summary>
    public interface IProcedureModule : IModule
    {
        /// <summary>
        /// 有限状态机
        /// </summary>
        IFsm Fsm { get; }

        /// <summary>
        /// 重定向启动流程
        /// </summary>
        /// <param name="name">流程类全名称</param>
        void Redirect(string name);

        /// <summary>
        /// 重定向启动流程
        /// </summary>
        /// <param name="type">流程类</param>
        void Redirect(Type type);

        /// <summary>
        /// 添加流程类
        /// </summary>
        /// <param name="type">流程类</param>
        void Add(Type type);

        /// <summary>
        /// 添加流程类
        /// </summary>
        /// <typeparam name="T">流程类</typeparam>
        void Add<T>() where T : ProcedureBase;
    }
}
