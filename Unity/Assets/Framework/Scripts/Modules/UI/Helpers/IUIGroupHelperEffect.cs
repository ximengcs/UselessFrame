using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UselessFrame.UIElements;

namespace UnityXFrameLib.UIElements
{
    /// <summary>
    /// UI组辅助器效果
    /// </summary>
    public interface IUIGroupHelperEffect
    {
        /// <summary>
        /// 播放效果
        /// </summary>
        /// <param name="ui">UI</param>
        /// <param name="onComplete">回调</param>
        UniTask<bool> Do(IUI ui, CancellationToken token);

        /// <summary>
        /// 停止并销毁效果
        /// </summary>
        /// <param name="ui">UI</param>
        bool Kill(IUI ui);

        void OnUpdate();
    }
}