
using UselessFrame.Runtime.Pools;

namespace UselessFrame.Runtime.Pools
{
    /// <summary>
    /// 可池化对象基类
    /// </summary>
    public abstract class PoolObjectBase : IPoolObject
    {
        /// <inheritdoc/>
        public int PoolKey { get; protected set; }

        IPool IPoolObject.InPool { get; set; }

        void IPoolObject.OnCreate()
        {
            OnCreateFromPool();
        }

        void IPoolObject.OnDelete()
        {
            OnDestroyFromPool();
        }

        void IPoolObject.OnRelease()
        {
            OnReleaseFromPool();
        }

        void IPoolObject.OnRequest()
        {
            OnRequestFromPool();
        }

        /// <summary>
        /// 第一次创建时的生命周期
        /// </summary>
        protected internal virtual void OnCreateFromPool() { }

        /// <summary>
        /// 从对象池中被请求时的生命周期
        /// </summary>
        protected internal virtual void OnRequestFromPool() { }

        /// <summary>
        /// 从对象池中销毁的生命周期
        /// </summary>
        protected internal virtual void OnDestroyFromPool() { }

        /// <summary>
        /// 释放到对象池中时的生命周期
        /// </summary>
        protected internal virtual void OnReleaseFromPool() { }
    }
}
