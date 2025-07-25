using System;
using XFrame.Core;
using XFrame.Collections;
using XFrame.Modules.ID;
using System.Collections.Generic;
using XFrame.Modules.Diagnotics;
using System.Text;
using XFrame.Modules.Pools;
using System.Threading;
using System.ComponentModel;

namespace XFrame.Modules.Containers
{
    /// <inheritdoc/>
    [CommonModule]
    [XType(typeof(IContainerModule))]
    public partial class ContainerModule : ModuleBase, IContainerModule
    {
        //private XCollection<IContainer> m_Containers;
        private Dictionary<int, IContainer> m_Containers;
        private Dictionary<int, IContainer> m_UpdateList;
        private List<IContainer> m_ContainersList;
        private List<IContainer> m_Cache;

        public void GetAll(List<IContainer> list)
        {
            list.AddRange(m_Containers.Values);
        }

        /// <inheritdoc/>
        public IContainer Get(int id)
        {
            if (m_Containers.TryGetValue(id, out IContainer container))
            {
                return container;
            }
            else
            {
                return null;
            }
        }

        public IContainer Create(int id, IContainerSetting setting)
        {
            return InnerNew(setting.Type, id, setting.ModuleUpdate, setting.Master, setting.DataProvider);
        }

        public IContainer Create(IContainerSetting setting)
        {
            int id = GetUseModule<IIdModule>().Next();
            return InnerNew(setting.Type, id, setting.ModuleUpdate, setting.Master, setting.DataProvider);
        }

        private IContainer InnerNew(Type type, int id, bool updateTrusteeship, IContainer master, OnDataProviderReady onReady)
        {
            IContainer container = Domain.TypeModule.CreateInstance(type) as IContainer;
            m_Containers.Add(id, container);
            m_ContainersList.Add(container);
            if (updateTrusteeship)
                m_UpdateList.Add(id, container);

            container.OnInit(this, id, master, onReady);
            Log.Debug(Log.Container, $"({Id})there is container added. {container.GetType().Name} {container.Id}");
            InnerDebugContainers();
            return container;
        }

        /// <inheritdoc/>
        public void Remove(IContainer container)
        {
            Log.Debug(Log.Container, $"({Id})there is container removed. {container.GetType().Name} {container.Id}");
            InnerRemoveRecursive(container);
            InnerDebugContainers();
        }

        public void Remove(int id)
        {
            IContainer container = Get(id);
            Remove(container);
        }

        private void InnerDebugContainers()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            foreach (IContainer child in m_Containers.Values)
            {
                bool root = child.Parent == null;
                if (root)
                {
                    sb.AppendLine($"- {child.GetType().Name} {child.Id} -> {(child.Parent != null ? child.Parent.Id.ToString() : "NULL")}");
                    InnerCheckRecursive(child, sb, "\t");
                }
                else
                {
                    sb.AppendLine($"· [{child.Id}|{child.GetType().Name}]");
                }
            }
            sb.Insert(0, $"({Id})");
            Log.Debug(Log.Container, sb.ToString());
        }

        private void InnerCheckRecursive(IContainer container, StringBuilder sb, string tab)
        {
            if (container is ShareContainer)
                return;
            tab += '\t';
            foreach (IContainer child in container)
            {
                if (child == container)
                {
                    Log.Error(Log.Container, $"({Id})same container, {child.GetType().Name} {container.GetType().Name}");
                    continue;
                }
                sb.Append(tab);
                sb.AppendLine($"{child.GetType().Name} {child.Id} -> {(child.Parent != null ? child.Parent.Id.ToString() : "NULL")}");
                InnerCheckRecursive(child, sb, tab);
            }
        }

        private void InnerRemoveRecursive(IContainer container)
        {
            List<IContainer> cache = new List<IContainer>();
            cache.Clear();
            foreach (IContainer child in container)
                cache.Add(child);
            foreach (IContainer child in cache)
                InnerRemoveRecursive(child);
            if (m_UpdateList.ContainsKey(container.Id))
                m_UpdateList.Remove(container.Id);
            if (m_ContainersList.Contains(container))
            {
                m_ContainersList.Remove(container);
            }
            if (m_Containers.ContainsKey(container.Id))
            {
                m_Containers.Remove(container.Id);
                container.OnDestroy();
            }
        }

        /// <inheritdoc/>
        protected override void OnInit(object data)
        {
            base.OnInit(data);
            m_Cache = new List<IContainer>(1024);
            m_Containers = new Dictionary<int, IContainer>();
            m_UpdateList = new Dictionary<int, IContainer>();
            m_ContainersList = new List<IContainer>();
        }

        /// <inheritdoc/>
        public void OnUpdate(double escapeTime)
        {
            m_Cache.Clear();
            m_Cache.AddRange(m_ContainersList);
            foreach (IContainer container in m_Cache)
            {
                if (container.Active)
                    container.OnUpdate(escapeTime);
            }
        }

        /// <inheritdoc/>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            m_Cache.Clear();
            m_Cache.AddRange(m_ContainersList);
            var it = new ListExt.BackwardIt<IContainer>(m_Cache);
            while (it.MoveNext())
            {
                IContainer container = it.Current;
                if (container == null)
                    continue;
                container.OnDestroy();
            }
            m_Containers = null;
            m_UpdateList = null;
            m_ContainersList = null;
        }
    }
}
