
using System;

namespace XFrame.Modules.Containers
{
    public struct ContainerSetting : IContainerSetting
    {
        private Type m_Type;

        public IContainer Master { get; set; }
        public bool ModuleUpdate { get; set; }
        public Type Type
        {
            get
            {
                if (m_Type == null)
                    return typeof(Container);
                else
                    return m_Type;
            }
        }
        public OnDataProviderReady DataProvider { get; set; }

        public ContainerSetting(Type type)
        {
            Master = null;
            m_Type = type;
            ModuleUpdate = true;
            DataProvider = null;
        }

        public ContainerSetting(Type type, IContainer master)
        {
            Master = master;
            m_Type = type;
            ModuleUpdate = true;
            DataProvider = null;
        }
    }
}
