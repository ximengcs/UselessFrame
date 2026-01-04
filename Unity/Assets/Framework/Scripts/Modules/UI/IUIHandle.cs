
using Cysharp.Threading.Tasks;
using UselessFrame.Runtime.Observable;

namespace UselessFrame.UIElements
{
    public interface IUIHandle
    {
        IUI UI { get; }

        IReadonlySubject<UIState> State { get; }

        UniTask WaitStateTask(UIState state);

        UniTask<IUI> WaitUI();
    }
}
