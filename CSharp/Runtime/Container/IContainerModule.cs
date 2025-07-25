
using System;
using System.Collections.Generic;
using XFrame.Core;

namespace XFrame.Modules.Containers
{
    /// <summary>
    /// 容器类模块
    /// </summary>
    public interface IContainerModule : IModule, IUpdater
    {
        /// <summary>
        /// 根据Id获取一个容器
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>容器</returns>
        IContainer Get(int id);

        IContainer Create(int id, IContainerSetting setting);

        IContainer Create(IContainerSetting setting);

        void GetAll(List<IContainer> result);

        /// <summary>
        /// 移除一个容器
        /// </summary>
        /// <param name="container">容器实例</param>
        void Remove(IContainer container);

        void Remove(int id);
    }
}
