
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace UselessFrame.ResourceManager
{
    public partial class ResourcesHelper
    {
        private struct AsyncState
        {
            private ResourceRequest _request;

            public AsyncState(ResourceRequest request)
            {
                _request = request;
            }

            public async UniTask Load()
            {
                _request.completed += CompleteHandler;
            }

            private void CompleteHandler(AsyncOperation asyncOp)
            {

            }
        }
    }
}
