
using System;
using Cysharp.Threading.Tasks;

namespace UselessFrame.Runtime.Pools
{
    public interface IPoolManager
    {
        IPoolObject Require(Type type, int poolKey = default, object userData = default);

        UniTask<IPoolObject> RequireAsync(Type type, int poolKey = default, object userData = default);

        T Require<T>(int poolKey = default, object userData = default) where T : IPoolObject;

        UniTask<T> RequireAsync<T>(int poolKey = default, object userData = default) where T : IPoolObject;

        void Release(IPoolObject inst);

        IPool<T> GetOrNew<T>() where T : IPoolObject;

        IPool GetOrNew(Type objType);

        void RegisterHelper<T>(IPoolHelper helper) where T : IPoolObject;

        void RegisterCommonHelper<T>(IPoolHelper helper) where T : IPoolObject;
    }
}
