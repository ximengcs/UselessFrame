
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityXFrame.Core.Diagnotics;
using UselessFrame.NewRuntime;
using UselessFrame.Runtime.Observable;
using UselessFrame.Runtime.Pools;

namespace UselessFrame.UIElements
{
    internal class UIHandle : IUIHandle, IPoolObject
    {
        private long _id;
        private IUI _ui;
        private Type _uiType;
        private UIAttribute _attr;
        private UIGroup _group;
        private Subject<UIHandle, UIState> _state;
        private CancellationTokenSource _openTokenSource;
        private CancellationTokenSource _closeTokenSource;
        private bool _closeToDestory;

        public IUI UI => _ui;

        IReadonlySubject<UIState> IUIHandle.State => _state;

        public ISubject<UIHandle, UIState> State => _state;

        public CancellationToken OpenToken => _openTokenSource?.Token ?? CancellationToken.None;

        public CancellationToken CloseToken => _closeTokenSource?.Token ?? CancellationToken.None;

        public bool Valid
        {
            get
            {
                switch (_state.Value)
                {
                    case UIState.Ready:
                    case UIState.Loading: return false;

                    default: return true;
                }
            }
        }

        int IPoolObject.PoolKey => GetPoolKey(_uiType);

        IPool IPoolObject.InPool { get; set; }

        public static int GetPoolKey(Type type)
        {
            return type.GetHashCode();
        }

        public void BindId(long id)
        {
            _id = id;
        }

        public void TryToPool()
        {
            if (!_closeToDestory)
                return;
            CancelCurrentOperate();

            if (_ui is IUIGroupElement groupElement)
            {
                _group.RemoveUI(groupElement);
                groupElement.OnSetGroup(null);
            }
            Debug.Log($"TryToPool 1");
            if (_ui is IPoolUI poolUI)
            {
                Debug.Log($"TryToPool 2");
                X.Pool.Release(poolUI);
            }
            X.Pool.Release(this);
        }

        public bool InGroup(string groupName)
        {
            if (_group == null)
                return false;
            return _group.Name == groupName;
        }

        public void SetGroup(UIGroup group)
        {
            _group = group;
            switch (_state.Value)
            {
                case UIState.Ready:
                case UIState.Loading:
                    _group = group;
                    break;

                default:
                    InnerSetUIGroup();
                    break;
            }
        }

        private void InnerSetUIGroup()
        {
            if (_ui is IUIGroupElement groupElement)
            {
                _group.AddUI(groupElement);
            }
        }

        private void CancelCurrentOperate()
        {
            if (_closeTokenSource != null && !_closeTokenSource.IsCancellationRequested)
                _closeTokenSource.Cancel();

            if (_openTokenSource != null && !_openTokenSource.IsCancellationRequested)
                _openTokenSource.Cancel();
        }

        public void TriggerOpen(object userData)
        {
            CancelCurrentOperate();
            _openTokenSource = new CancellationTokenSource();
            InnerTriggerOpen(userData, OpenToken);
        }

        private async void InnerTriggerOpen(object userData, CancellationToken token)
        {
            if (_state.Value == UIState.Ready)
            {
                _state.Value = UIState.Loading;
                IUI loadUI = (IUI)await X.Pool.RequireAsync(_uiType, _attr?.ResSource ?? 0);
                if (token.IsCancellationRequested)
                {
                    if (loadUI is IPoolUI poolUI)
                        X.Pool.Release(poolUI);
                    return;
                }
                _ui = loadUI;
                if (_ui is IUIGroupElement groupElement)
                    groupElement.OnBindHandle(_id, this);
                _state.Value = UIState.Loaded;
            }

            if (_state.Value == UIState.Loaded)
            {
                _state.Value = UIState.Initing;
                if (_ui is IUIGroupElement groupElement)
                {
                    InnerSetUIGroup();
                    groupElement.OnInit(userData);
                }
            }

            switch (_state.Value)
            {
                case UIState.Initing:
                case UIState.Close:
                case UIState.CloseBegin:
                case UIState.CloseEnd:
                case UIState.Open:
                case UIState.OpenBegin:
                case UIState.OpenEnd:
                    _ui.Open();
                    break;
            }
        }

        public void TriggerClose(bool destroy)
        {
            _closeToDestory = destroy;
            CancelCurrentOperate();

            _closeTokenSource = new CancellationTokenSource();
            switch (_state.Value)
            {
                case UIState.Initing:
                case UIState.OpenBegin:
                case UIState.Open:
                case UIState.OpenEnd:
                    {
                        _ui.Close();
                        break;
                    }

                case UIState.CloseEnd:
                    {
                        TryToPool();
                        break;
                    }
            }
        }

        public void OpenFinish()
        {
            _state.Value = UIState.Open;
            if (_ui is IUIGroupElement groupElement)
                groupElement.OnOpen();
            _state.Value = UIState.OpenEnd;
        }

        public void CloseFinish()
        {
            _state.Value = UIState.Close;
            if (_ui is IUIGroupElement groupElement)
                groupElement.OnClose();
            _state.Value = UIState.CloseEnd;
            TryToPool();
        }

        public async UniTask<IUI> WaitUI()
        {
            switch (_state.Value)
            {
                case UIState.Ready:
                    {
                        TaskInfo info = new TaskInfo();
                        await info.Start(_state, UIState.Loading);
                        break;
                    }
            }

            return _ui;
        }

        public UniTask WaitStateTask(UIState state)
        {
            TaskInfo info = new TaskInfo();
            return info.Start(_state, state);
        }

        void IPoolObject.OnCreate(object userData)
        {
            _uiType = (Type)userData;
            _attr = (UIAttribute)X.Type.GetAttribute(_uiType, typeof(UIAttribute));
            _state = new Subject<UIHandle, UIState>(this, UIState.Ready);
            _state.Subscribe(StateChangeHandler);
        }

        private void StateChangeHandler(UIState oldState, UIState newState)
        {
            X.Log.Debug(UnityXFrame.Core.Diagnotics.LogSort.UI, $"[{_uiType.Name}][{_id}]state : {oldState} -> {newState}");
        }

        void IPoolObject.OnRequest()
        {
        }

        void IPoolObject.OnRelease()
        {
            _ui = null;
            _openTokenSource = null;
            _closeTokenSource = null;
            _state.Value = UIState.Ready;
        }

        void IPoolObject.OnDelete()
        {
        }

        private class TaskInfo
        {
            private UniTaskCompletionSource _source;
            private IReadonlySubject<UIState> _subject;
            private UIState _state;
            private bool _complete;

            public async UniTask Start(IReadonlySubject<UIState> subject, UIState state)
            {
                _state = state;
                _subject = subject;
                _source = new UniTaskCompletionSource();
                Debug.Log($"[State][Subscribe]StateChangeHandler {state} {_source.GetHashCode()} {_complete}");
                _subject.Unsubscribe(StateChangeHandler);
                _subject.Subscribe(StateChangeHandler);
                await _source.Task;
            }

            private void StateChangeHandler(UIState state)
            {
                if (_state == state)
                {
                    _subject.Unsubscribe(StateChangeHandler);
                    Debug.Log($"[State][UnSubscribe]StateChangeHandler {state} {_source.GetHashCode()} {_complete}");
                    _source.TrySetResult();
                    _complete = true;
                }
            }
        }
    }
}
