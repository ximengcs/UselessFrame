
using System;
using System.Collections.Generic;
using UselessFrame.Runtime.Types;

namespace UselessFrame.Runtime.Pools
{
    internal partial class PoolSystem : IPoolSystem
    {
        private object[] m_ParamCache;
        private DefaultPoolHelper _defaultHelper;
        private Dictionary<Type, IPool> m_PoolContainers;
        private IFrameCore _core;

        public PoolSystem(IFrameCore core)
        {
            _core = core;
            m_ParamCache = new object[2];
            m_PoolContainers = new Dictionary<Type, IPool>();

            Type helperType = typeof(IPoolHelper);
            Type helperPType = typeof(PoolHelperAttribute);
            ITypeCollection typeSys = _core.TypeSystem.GetOrNewWithAttr(helperPType);
            foreach (Type type in typeSys)
            {
                if (helperType.IsAssignableFrom(type))
                {
                    PoolHelperAttribute attr = (PoolHelperAttribute)_core.TypeSystem.GetAttribute(type, helperPType);
                    IPoolHelper helper = _core.TypeSystem.CreateInstance(type) as IPoolHelper;
                    InnerGetOrNew(attr.Target, helper);
                }
            }
        }

        public void Dispose()
        {
            foreach (IPool pool in m_PoolContainers.Values)
                pool.ClearObject();
            m_PoolContainers = null;
        }

        #region Interface
        /// <inheritdoc/>
        public IPool<T> GetOrNew<T>(IPoolHelper helper = null) where T : IPoolObject
        {
            return InnerGetOrNew(typeof(T), helper) as IPool<T>;
        }

        /// <inheritdoc/>
        public IPool GetOrNew(Type objType, IPoolHelper helper = null)
        {
            return InnerGetOrNew(objType, helper);
        }
        #endregion

        #region Inner Implement
        internal IPool InnerGetOrNew(Type objType, IPoolHelper helper = null)
        {
            if (!m_PoolContainers.TryGetValue(objType, out IPool pool))
            {
                Type poolType = typeof(ObjectPool<>).MakeGenericType(objType);
                m_ParamCache[0] = this;
                m_ParamCache[1] = _defaultHelper;
                pool = _core.TypeSystem.CreateInstance(poolType, m_ParamCache) as IPool;

                m_PoolContainers.Add(objType, pool);
            }

            return pool;
        }
        #endregion
    }
}
