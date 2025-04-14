
using System;
using UselessFrame.Runtime.Types;

namespace UselessFrame.Runtime.Pools
{
    internal partial class PoolSystem
    {
        private class DefaultPoolHelper : IPoolHelper
        {
            private ITypeSystem _typeSys;

            public int CacheCount => 64;

            public DefaultPoolHelper(ITypeSystem typeSys)
            {
                _typeSys = typeSys;
            }

            IPoolObject IPoolHelper.Factory(Type type, int poolKey, object userData)
            {
                return (IPoolObject)_typeSys.CreateInstance(type);
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
