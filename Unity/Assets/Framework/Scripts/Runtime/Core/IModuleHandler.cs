using Cysharp.Threading.Tasks;
using System;

namespace UselessFrame.Runtime
{
    /// <summary>
    /// 模块处理器
    /// </summary>
    public interface IModuleHandler
    {
        /// <summary>
        /// 目标类型
        /// </summary>
        Type Target { get; }

        /// <summary>
        /// 处理函数,当实现此类型并且处理器注册到核心时,此方法会被执行
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="data">参数</param>
        virtual void Handle(IModule module, object data) { }

        virtual void Handle(IModule module, float data) { }
    }
}
