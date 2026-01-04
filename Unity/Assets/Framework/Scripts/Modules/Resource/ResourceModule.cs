using Cysharp.Threading.Tasks;
using UnityEngine;
using UselessFrame.Runtime;

namespace UselessFrame.ResourceManager
{
    public class ResourceModule : ModuleBase
    {
        public T Load<T>(string path)
        {
            return (T)(object)Resources.Load(path, typeof(T));
        }

        public async UniTask<T> LoadAsync<T>(string path)
        {
            ResourceRequest request = Resources.LoadAsync(path, typeof(T));
            UniTaskCompletionSource<T> source = new UniTaskCompletionSource<T>();
            request.completed += (op) => source.TrySetResult((T)(object)((ResourceRequest)op).asset);
            return await source.Task;
        }
    }
}
