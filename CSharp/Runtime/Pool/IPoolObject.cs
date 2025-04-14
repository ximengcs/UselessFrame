using System;

namespace UselessFrame.Runtime.Pools
{
    /// <summary>
    /// 可池化对象
    /// </summary>
    public interface IPoolObject
    {
        /// <summary>
        /// 对象Key，即使对象类型<see cref="Type"/>相同,当从对象池中获取对象时也会获取同key的对象。
        /// </summary>
        int PoolKey { get; }

        /// <summary>
        /// 对象名
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 对象所处对象池
        /// </summary>
        IPool InPool { get; protected internal set; }

        /// <summary>
        /// 从对象池中创建时被调用
        /// </summary>
        protected internal void OnCreate();

        /// <summary>
        /// 从对象池中请求时被调用
        /// </summary>
        protected internal void OnRequest();

        /// <summary>
        /// 释放到对象池中时被调用
        /// </summary>
        protected internal void OnRelease();

        /// <summary>
        /// 从对象池中销毁时被调用
        /// </summary>
        protected internal void OnDelete();
    }
}
