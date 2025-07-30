
using Cysharp.Threading.Tasks;

namespace UselessFrame.NewRuntime
{
    internal interface IManagerDisposable
    {
        UniTask Dispose();
    }
}
