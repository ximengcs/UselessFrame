
using Cysharp.Threading.Tasks;
using System;

namespace UselessFrame.Runtime
{
    public interface IModuleDriver
    {
        IModule Get(Type type, int id = default);

        UniTask<IModule> Add(Type type, object param = null, int id = default);

        void Remove(Type type, int id = default);
    }
}
