
namespace UselessFrame.NewRuntime.StateMachine
{
    /// <summary>
    /// 有限状态机
    /// </summary>
    public interface IFsmBase
    {
        /// <summary>
        /// 状态机名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 初始化生命周期
        /// </summary>
        protected internal void OnInit();

        /// <summary>
        /// 更新生命周期
        /// </summary>
        protected internal void OnUpdate();

        /// <summary>
        /// 销毁生命周期
        /// </summary>
        protected internal void OnDestroy();
    }
}
