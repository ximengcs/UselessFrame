using System;
using System.Collections.Generic;
using UselessFrame.Runtime.Collections;

namespace XFrame.Modules.Containers
{
    /// <summary>
    /// 容器
    /// </summary>
    public interface IContainer : IXItem, IDataProvider, IXEnumerable<IContainer>
    {
        bool Active { get; }

        /// <summary>
        /// 根容器
        /// </summary>
        IContainer Master { get; }

        /// <summary>
        /// 父容器
        /// </summary>
        IContainer Parent { get; }

        /// <summary>
        /// 初始化生命周期
        /// </summary>
        /// <param name="module">容器所属模块</param>
        /// <param name="id">容器Id</param>
        /// <param name="master">容器拥有者</param>
        /// <param name="onReady">容器就绪事件</param>
        void OnInit(IContainerModule module, int id, IContainer master, OnDataProviderReady onReady);

        /// <summary>
        /// 更新生命周期
        /// </summary>
        /// <param name="elapseTime">逃逸时间</param>
        void OnUpdate(double elapseTime);

        /// <summary>
        /// 销毁生命周期
        /// </summary>
        void OnDestroy();

        void SetActive(bool active, bool recursive = true);

        /// <summary>
        /// 获取一个组件(Id为默认Id)
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="id">组件Id</param>
        /// <returns>组件实例</returns>
        T GetCom<T>(int id = default, bool useXType = true) where T : IContainer;

        /// <summary>
        /// 获取一个组件
        /// </summary>
        /// <param name="type">组件类型</param>
        /// <param name="id">组件Id</param>
        /// <returns>组件实例</returns>
        IContainer GetCom(Type type, int id = default, bool useXType = true);

        List<T> GetComs<T>(bool useXType = false) where T : IContainer;

        List<IContainer> GetComs(Type targetType, bool useXType = false);

        /// <summary>
        /// 添加一个组件(Id为默认Id)
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="onReady">初始化完成事件</param>
        /// <returns>添加的组件</returns>
        T AddCom<T>(OnDataProviderReady onReady = null) where T : IContainer;

        /// <summary>
        /// 添加一个组件
        /// </summary>
        /// <param name="com">组件实例</param>
        /// <returns>组件实例</returns>
        IContainer AddCom(IContainer com);

        /// <summary>
        /// 添加一个组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="id">组件Id</param>
        /// <param name="onReady">初始化完成事件</param>
        /// <returns>添加的组件</returns>
        T AddCom<T>(int id, OnDataProviderReady onReady = null) where T : IContainer;

        /// <summary>
        /// 添加一个组件(Id为默认Id)
        /// </summary>
        /// <param name="type">组件类型</param>
        /// <param name="onReady">初始化完成事件</param>
        /// <returns>添加的组件</returns>
        IContainer AddCom(Type type, OnDataProviderReady onReady = null);

        /// <summary>
        /// 添加一个组件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="onReady">初始化完成事件</param>
        /// <returns>添加的组件</returns>
        IContainer AddCom(Type type, int id, OnDataProviderReady onReady = null);

        /// <summary>
        /// 获取或添加一个组件(Id为默认Id)
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="onReady">初始化完成事件</param>
        /// <returns>添加的组件</returns>
        T GetOrAddCom<T>(OnDataProviderReady onReady = null) where T : IContainer;

        /// <summary>
        /// 获取或添加一个组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="id">组件Id</param>
        /// <param name="onReady">初始化完成事件</param>
        /// <returns>添加的组件</returns>
        T GetOrAddCom<T>(int id, OnDataProviderReady onReady = null) where T : IContainer;

        /// <summary>
        /// 获取或添加一个组件(Id为默认Id)
        /// </summary>
        /// <param name="type">组件类型</param>
        /// <param name="onReady">初始化完成事件</param>
        /// <returns>添加的组件</returns>
        IContainer GetOrAddCom(Type type, OnDataProviderReady onReady = null);

        /// <summary>
        /// 获取或添加一个组件
        /// </summary>
        /// <param name="type">组件类型</param>
        /// <param name="id">组件Id</param>
        /// <param name="onReady">初始化完成事件</param>
        /// <returns>添加的组件</returns>
        IContainer GetOrAddCom(Type type, int id, OnDataProviderReady onReady = null);

        /// <summary>
        /// 移除组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="id">组件Id</param>
        void RemoveCom<T>(int id = default) where T : IContainer;

        /// <summary>
        /// 移除组件
        /// </summary>
        /// <param name="type">组件类型</param>
        /// <param name="id">组件Id</param>
        void RemoveCom(Type type, int id = default);

        /// <summary>
        /// 移除所有组件
        /// </summary>
        void ClearCom();
    }
}
