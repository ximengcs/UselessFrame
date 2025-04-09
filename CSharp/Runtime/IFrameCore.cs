
using Cysharp.Threading.Tasks;

namespace UselessFrame.Runtime
{
    public interface IFrameCore : IModuleDriver
    {
        int Id { get; }

        UniTask Start();

        UniTask Destroy();
    }
}
