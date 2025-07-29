
using System;
using UselessFrame.NewRuntime;

namespace UselessFrame.Runtime.Pools
{
    internal partial class PoolManager
    {
        private class DefaultPoolHelper : IPoolHelper
        {
            public int CacheCount => 64;

            IPoolObject IPoolHelper.Factory(Type type, int poolKey)
            {
                return (IPoolObject)X.Type.CreateInstance(type);
            }

            void IPoolHelper.OnObjectCreate(IPoolObject obj)
            {

            }

            void IPoolHelper.OnObjectDestroy(IPoolObject obj)
            {

            }

            void IPoolHelper.OnObjectRelease(IPoolObject obj)
            {

            }

            void IPoolHelper.OnObjectRequest(IPoolObject obj)
            {

            }
        }
    }
}
