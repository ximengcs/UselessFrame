
using System.Collections.Generic;
using System;

namespace UselessFrame.Runtime.Pools
{
    /// <summary>
    /// 对象池
    /// </summary>
    public interface IPool
    {
        /// <summary>
        /// 对象池持有类型
        /// </summary>
        Type ObjectType { get; }

        /// <summary>
        /// 当前池中对象数量
        /// </summary>
        int ObjectCount { get; }

        /// <summary>
        /// 使用中数量
        /// </summary>
        int UseCount { get; }

        /// <summary>
        /// 对象池辅助器
        /// </summary>
        IPoolHelper Helper { get; }

        /// <summary>
        /// 所属模块
        /// </summary>
        IPoolSystem System { get; }

        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <param name="poolKey">对象key</param>
        /// <param name="userData">对象数据</param>
        /// <returns>对象实例</returns>
        IPoolObject Require(int poolKey = default, object userData = default);

        /// <summary>
        /// 释放一个对象 
        /// </summary>
        /// <param name="obj">待释放的对象</param>
        void Release(IPoolObject obj);

        /// <summary>
        /// 生成池对象
        /// </summary>
        /// <param name="poolKey">对象key</param>
        /// <param name="count">生成数量</param>
        /// <param name="userData">数据参数</param>
        /// <param name="toList">添加到列表</param>
        void Spawn(int poolKey = default, int count = 1, object userData = default, List<IPoolObject> toList = null);

        /// <summary>
        /// 清除所有池化对象
        /// </summary>
        void ClearObject();
    }

    /// <summary>
    /// 对象池
    /// </summary>
    /// <typeparam name="T">持有对象类型</typeparam>
    public interface IPool<T> : IPool where T : IPoolObject
    {
        /// <summary>
        /// 释放一个对象
        /// </summary>
        /// <param name="obj">要释放的对象</param>
        void Release(T obj);

        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <param name="poolKey">对象key</param>
        /// <param name="userData">数据参数</param>
        /// <returns>是否是新创建的对象，返回false表示从对象池中创建</returns>
        new T Require(int poolKey = default, object userData = default);
    }
}
