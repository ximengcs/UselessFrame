﻿
using System;

namespace UselessFrame.Runtime.Pools
{
    public interface IPoolManager
    {
        T Require<T>(int poolKey = default, object userData = default) where T : IPoolObject;

        void Release(IPoolObject inst);

        IPool<T> GetOrNew<T>() where T : IPoolObject;

        IPool GetOrNew(Type objType);

        void RegisterHelper<T>(IPoolHelper helper) where T : IPoolObject;
    }
}
