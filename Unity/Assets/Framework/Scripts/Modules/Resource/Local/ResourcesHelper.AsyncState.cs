
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace UselessFrame.ResourceManager
{
    public partial class ResourcesHelper
    {
        private struct AsyncState
        {
            private ResourceRequest _request;
            private bool _complete;
            private AutoResetUniTaskCompletionSource _taskSource;

            public AsyncState(ResourceRequest request)
            {
                _request = request;
                _complete = false;
                _taskSource = AutoResetUniTaskCompletionSource.Create();
                _request.completed += CompleteHandler;
            }

            public async UniTask<object> Load()
            {
                if (!_complete)
                {
                    await _taskSource.Task;
                }
                return _request.asset;
            }

            private void CompleteHandler(AsyncOperation asyncOp)
            {
                if (_complete)
                    return;
                _complete = true;
                _request.completed -= CompleteHandler;
                _taskSource.TrySetResult();
            }
        }
    }
}
