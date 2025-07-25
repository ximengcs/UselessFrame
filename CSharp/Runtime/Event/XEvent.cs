using UselessFrame.Runtime.Pools;

namespace UselessFrame.NewRuntime.Events
{
    /// <summary>
    /// 事件
    /// </summary>
    public abstract class XEvent : PoolObjectBase, IPoolObject
    {
        /// <summary>
        /// 事件Id 
        /// </summary>
        public int Id { get; protected set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="id">事件Id</param>
        public XEvent(int id)
        {
            Id = id;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        public XEvent() { }

        /// <inheritdoc/>
        protected internal override void OnReleaseFromPool()
        {
            base.OnReleaseFromPool();
            Id = default;
        }
    }
}
