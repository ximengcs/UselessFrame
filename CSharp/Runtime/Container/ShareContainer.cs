using System;
using XFrame.Collections;
using System.Collections.Generic;

namespace XFrame.Modules.Containers
{
    /// <summary>
    /// 共享组件基类, 会共享容器数据
    /// </summary>
    public abstract class ShareContainer : IContainer
    {
        private IContainer m_Owner;
        private bool m_Active;

        /// <summary>
        /// 所属容器模块
        /// </summary>
        protected IContainerModule m_Module;

        /// <summary>
        /// 激活状态
        /// </summary>
        public bool Active
        {
            get { return m_Active; }
        }

        public void SetActive(bool active, bool recursive = true)
        {
            if (m_Active != active)
            {
                m_Active = active;
                if (m_Active)
                    OnActive();
                else
                    OnInactive();
            }
        }

        /// <inheritdoc/>
        public int Id { get; private set; }

        /// <inheritdoc/>
        public IContainer Master => m_Owner.Master;

        /// <summary>
        /// 父容器
        /// </summary>
        public IContainer Parent
        {
            get => m_Owner.Parent;
        }

        void IContainer.OnInit(IContainerModule module, int id, IContainer master, OnDataProviderReady onReady)
        {
            Id = id;
            m_Module = module;
            m_Owner = master;
            if (master != null)
                master.AddCom(this);

            onReady?.Invoke(this);
            OnInit();
        }

        /// <summary>
        /// 初始化生命周期
        /// </summary>
        protected internal virtual void OnInit() { }

        void IContainer.OnUpdate(double elapseTime)
        {
            OnUpdate(elapseTime);
        }

        /// <summary>
        /// 更新生命周期
        /// </summary>
        /// <param name="elapseTime">逃逸时间</param>
        protected internal virtual void OnUpdate(double elapseTime) { }

        void IContainer.OnDestroy()
        {
            OnDestroy();
            m_Owner = null;
            m_Module = null;
        }

        /// <summary>
        /// 销毁生命周期
        /// </summary>
        protected internal virtual void OnDestroy() { }

        /// <summary>
        /// 激活生命周期
        /// </summary>
        protected virtual void OnActive() { }

        /// <summary>
        /// 失活生命周期
        /// </summary>
        protected virtual void OnInactive() { }

        /// <inheritdoc/>
        public T GetCom<T>(int id = 0, bool useXType = true) where T : IContainer
        {
            return m_Owner.GetCom<T>(id, useXType);
        }

        /// <inheritdoc/>
        public IContainer GetCom(Type type, int id = 0, bool useXType = true)
        {
            return m_Owner.GetCom(type, id, useXType);
        }

        /// <inheritdoc/>
        public T AddCom<T>(OnDataProviderReady onReady = null) where T : IContainer
        {
            return m_Owner.AddCom<T>(onReady);
        }

        /// <inheritdoc/>
        public IContainer AddCom(IContainer com)
        {
            return m_Owner.AddCom(com);
        }

        /// <inheritdoc/>
        public T AddCom<T>(int id, OnDataProviderReady onReady = null) where T : IContainer
        {
            return m_Owner.AddCom<T>(id, onReady);
        }

        /// <inheritdoc/>
        public IContainer AddCom(Type type, OnDataProviderReady onReady = null)
        {
            return m_Owner.AddCom(type, onReady);
        }

        /// <inheritdoc/>
        public IContainer AddCom(Type type, int id, OnDataProviderReady onReady = null)
        {
            return m_Owner.AddCom(type, id, onReady);
        }

        /// <inheritdoc/>
        public T GetOrAddCom<T>(OnDataProviderReady onReady = null) where T : IContainer
        {
            return m_Owner.GetOrAddCom<T>(onReady);
        }

        /// <inheritdoc/>
        public T GetOrAddCom<T>(int id, OnDataProviderReady onReady = null) where T : IContainer
        {
            return m_Owner.GetOrAddCom<T>(id, onReady);
        }

        /// <inheritdoc/>
        public IContainer GetOrAddCom(Type type, OnDataProviderReady onReady = null)
        {
            return m_Owner.GetOrAddCom(type, onReady);
        }

        /// <inheritdoc/>
        public IContainer GetOrAddCom(Type type, int id, OnDataProviderReady onReady = null)
        {
            return m_Owner.GetOrAddCom(type, id, onReady);
        }

        /// <inheritdoc/>
        public void RemoveCom<T>(int id = 0) where T : IContainer
        {
            m_Owner.RemoveCom<T>(id);
        }

        /// <inheritdoc/>
        public void RemoveCom(Type type, int id = 0)
        {
            m_Owner.RemoveCom(type, id);
        }

        /// <inheritdoc/>
        public void ClearCom()
        {
            m_Owner.ClearCom();
        }

        /// <inheritdoc/>
        public bool HasData<T>()
        {
            return m_Owner.HasData<T>();
        }

        /// <inheritdoc/>
        public bool HasData<T>(string name)
        {
            return m_Owner.HasData<T>(name);
        }

        /// <inheritdoc/>
        public void SetData<T>(T value)
        {
            m_Owner.SetData(value);
        }

        /// <inheritdoc/>
        public T GetData<T>()
        {
            return m_Owner.GetData<T>();
        }

        /// <inheritdoc/>
        public void SetData<T>(string name, T value)
        {
            m_Owner.SetData(name, value);
        }

        /// <inheritdoc/>
        public T GetData<T>(string name)
        {
            return m_Owner.GetData<T>(name);
        }

        /// <inheritdoc/>
        public void ClearData()
        {
            m_Owner.ClearData();
        }

        /// <inheritdoc/>
        public IEnumerator<IContainer> GetEnumerator()
        {
            return m_Owner.GetEnumerator();
        }

        /// <inheritdoc/>
        public void SetIt(XItType type)
        {
            m_Owner.SetIt(type);
        }

        public List<T> GetComs<T>(bool useXType = false) where T : IContainer
        {
            return m_Owner.GetComs<T>(useXType);
        }

        public List<IContainer> GetComs(Type targetType, bool useXType = false)
        {
            return m_Owner.GetComs(targetType, useXType);
        }
    }
}
