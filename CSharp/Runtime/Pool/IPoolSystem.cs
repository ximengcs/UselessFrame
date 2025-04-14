
using System;

namespace UselessFrame.Runtime.Pools
{
    public interface IPoolSystem : IFrameSystem
    {
        IPool<T> GetOrNew<T>(IPoolHelper helper = null) where T : IPoolObject;

        IPool GetOrNew(Type objType, IPoolHelper helper = null);
    }
}
