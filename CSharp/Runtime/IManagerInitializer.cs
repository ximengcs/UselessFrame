
using Cysharp.Threading.Tasks;

namespace UselessFrame.NewRuntime
{
    internal interface IManagerInitializer
    {
        UniTask Initialize(XSetting setting);
    }
}
