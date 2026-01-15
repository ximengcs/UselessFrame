
using Cysharp.Threading.Tasks;
using UselessFrame.Runtime;

namespace UselessFrame.ResourceManager
{
    public interface IResourceModule : IModule
    {
        T Load<T>(string path);

        UniTask<T> LoadAsync<T>(string path);
    }
}
