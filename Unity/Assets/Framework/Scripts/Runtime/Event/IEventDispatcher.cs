
namespace UselessFrame.NewRuntime.Events
{
    public interface IEventDispatcher
    {
        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="eventId">事件Id</param>
        void Trigger(int eventId);

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="e">事件</param>
        void Trigger(XEvent e);

        /// <summary>
        /// 立刻触发事件
        /// </summary>
        /// <param name="eventId">事件Id</param>
        void TriggerNow(int eventId);

        /// <summary>
        /// 立即触发事件
        /// </summary>
        /// <param name="e">事件</param>
        void TriggerNow(XEvent e);

        /// <summary>
        /// 监听事件
        /// </summary>
        /// <param name="eventId">事件Id</param>
        /// <param name="handler">事件处理委托</param>
        void Listen(int eventId, XEventHandler handler);

        /// <summary>
        /// 监听事件
        /// </summary>
        /// <param name="eventId">事件Id</param>
        /// <param name="handler">事件处理委托</param>
        void Listen(int eventId, XEventHandler2 handler);

        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventId">事件Id</param>
        /// <param name="handler">需要移除的委托</param>
        void Unlisten(int eventId, XEventHandler handler);

        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventId">事件Id</param>
        /// <param name="handler">需要移除的委托</param>
        void Unlisten(int eventId, XEventHandler2 handler);

        /// <summary>
        /// 移除事件的所有监听
        /// </summary>
        /// <param name="eventId">事件Id</param>
        void Unlisten(int eventId);

        /// <summary>
        /// 移除所有监听
        /// </summary>
        void Unlisten();
    }
}
