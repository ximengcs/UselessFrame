
using System;

namespace UselessFrame.Runtime.Pools
{
    public interface IPoolManager
    {
        IPool<T> GetOrNew<T>() where T : IPoolObject;

        IPool GetOrNew(Type objType);

        void RegisterHelper<T>(IPoolHelper helper) where T : IPoolObject;
    }
}
