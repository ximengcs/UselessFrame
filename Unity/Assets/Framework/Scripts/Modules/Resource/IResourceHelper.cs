
using Cysharp.Threading.Tasks;
using System;

namespace UselessFrame.ResourceManager
{
    public interface IResourceHelper
    {
        void SetResourceHelper(IResourceHelper helper);

        T Load<T>(string resPath);

        object Load(Type type, string resPath);

        UniTask<T> LoadAsync<T>(string resPath);

        UniTask<object> LoadAsync(Type type, string resPath);

        void Unload(object asset);

        void Unload();
    }
}
