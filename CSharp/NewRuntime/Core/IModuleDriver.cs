
using Cysharp.Threading.Tasks;
using System;

namespace UselessFrame.Runtime
{
    public interface IModuleDriver
    {
        IModule GetModule(Type type, int id = default);

        UniTask<IModule> AddModule(Type type, object param = null);

        UniTask RemoveModule(Type type, int id = default);
    }
}
