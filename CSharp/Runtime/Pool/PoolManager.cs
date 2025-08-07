
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UselessFrame.NewRuntime;
using UselessFrame.Runtime.Types;

namespace UselessFrame.Runtime.Pools
{
    internal partial class PoolManager : IPoolManager, IManagerInitializer
    {
        private object[] m_ParamCache;
        private DefaultPoolHelper _defaultHelper;
        private Dictionary<Type, IPoolHelper> _helpers;
        private Dictionary<Type, IPoolHelper> _commonHelpers;
        private Dictionary<Type, IPool> m_PoolContainers;

        public PoolManager()
        {
            m_ParamCache = new object[2];
            m_PoolContainers = new Dictionary<Type, IPool>();
            _defaultHelper = new DefaultPoolHelper();
            _helpers = new Dictionary<Type, IPoolHelper>();
            _commonHelpers = new Dictionary<Type, IPoolHelper>();
        }

        public async UniTask Initialize(XSetting setting)
        {
            Type helperType = typeof(IPoolHelper);
            Type helperPType = typeof(PoolHelperAttribute);
            ITypeCollection typeSet = X.Type.GetCollection(helperPType);
            foreach (Type type in typeSet)
            {
                if (helperType.IsAssignableFrom(type))
                {
                    PoolHelperAttribute attr = (PoolHelperAttribute)X.Type.GetAttribute(type, helperPType);
                    IPoolHelper helper = X.Type.CreateInstance(type) as IPoolHelper;
                    _helpers[attr.Target] = helper;
                }
            }
        }

        public async UniTask Dispose()
        {
            foreach (IPool pool in m_PoolContainers.Values)
                pool.ClearObject();
            m_PoolContainers = null;
        }

        #region Interface
        public void RegisterHelper<T>(IPoolHelper helper) where T : IPoolObject
        {
            _helpers[typeof(T)] = helper;
        }

        public void RegisterCommonHelper<T>(IPoolHelper helper) where T : IPoolObject
        {
            _commonHelpers[typeof(T)] = helper;
        }

        public IPoolObject Require(Type type, int poolKey = default, object userData = default)
        {
            IPool pool = GetOrNew(type);
            return pool.Require(poolKey, userData);
        }

        public UniTask<IPoolObject> RequireAsync(Type type, int poolKey = default, object userData = default)
        {
            IPool pool = GetOrNew(type);
            return pool.RequireAsync(poolKey, userData);
        }

        public T Require<T>(int poolKey = default, object userData = default) where T : IPoolObject
        {
            IPool<T> pool = GetOrNew<T>();
            return pool.Require(poolKey, userData);
        }

        public UniTask<T> RequireAsync<T>(int poolKey = default, object userData = default) where T : IPoolObject
        {
            IPool<T> pool = GetOrNew<T>();
            return pool.RequireAsync(poolKey, userData);
        }

        public void Release(IPoolObject inst)
        {
            IPool pool = GetOrNew(inst.GetType());
            pool.Release(inst);
        }

        /// <inheritdoc/>
        public IPool<T> GetOrNew<T>() where T : IPoolObject
        {
            return InnerGetOrNew(typeof(T)) as IPool<T>;
        }

        /// <inheritdoc/>
        public IPool GetOrNew(Type objType)
        {
            return InnerGetOrNew(objType);
        }
        #endregion

        #region Inner Implement
        internal IPool InnerGetOrNew(Type objType)
        {
            IPoolHelper helper = null;
            _helpers.TryGetValue(objType, out helper);
            
            if (helper == null)
            {
                foreach (var entry in _commonHelpers)
                {
                    if (entry.Key.IsAssignableFrom(objType))
                    {
                        helper = entry.Value;
                        break;
                    }
                }
            }

            if (helper == null)
            {
                helper = _defaultHelper;
            }

            if (!m_PoolContainers.TryGetValue(objType, out IPool pool))
            {
                Type poolType = typeof(ObjectPool<>).MakeGenericType(objType);
                m_ParamCache[0] = this;
                m_ParamCache[1] = helper;
                pool = X.Type.CreateInstance(poolType, m_ParamCache) as IPool;

                m_PoolContainers.Add(objType, pool);
            }

            return pool;
        }
        #endregion
    }
}
