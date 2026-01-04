
using Cysharp.Threading.Tasks;
using System;
using UselessFrame.Runtime.Observable;

namespace UselessFrame.UIElements
{
    internal class EmptyUIHandle : IUIHandle
    {
        private class Subject : IReadonlySubject<UIState>
        {
            public UIState Value { get; }

            public Subject(UIState value)
            {
                Value = value;
            }

            public void Subscribe(Action<UIState> changeHandler, bool onceTrigger = false)
            {
            }

            public void Subscribe(Action<UIState, UIState> changeHandler, bool onceTrigger = false)
            {
            }

            public void Unsubscribe(Action<UIState> changeHandler)
            {
            }

            public void Unsubscribe(Action<UIState, UIState> changeHandler)
            {
            }
        }

        public IUI UI => null;

        private Subject _subject = new Subject(UIState.Empty);
        public IReadonlySubject<UIState> State => _subject;

        public async UniTask WaitStateTask(UIState state)
        {
            await UniTask.CompletedTask;
        }

        public async UniTask<IUI> WaitUI()
        {
            await UniTask.CompletedTask;
            return null;
        }

        private static EmptyUIHandle _emptyUIHandle;
        public static EmptyUIHandle Default
        {
            get
            {
                if (_emptyUIHandle == null)
                    _emptyUIHandle = new EmptyUIHandle();
                return _emptyUIHandle;
            }
        }
    }
}
