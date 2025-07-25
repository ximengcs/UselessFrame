
using System;

namespace XFrame.Modules.Containers
{
    public interface IContainerSetting
    {
        public IContainer Master { get; }
        public bool ModuleUpdate { get; }
        public Type Type { get; }
        public OnDataProviderReady DataProvider { get; }
    }
}
