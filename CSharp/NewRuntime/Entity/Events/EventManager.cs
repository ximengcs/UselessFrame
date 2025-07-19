
using System;
using System.Collections.Generic;
using UselessFrame.Runtime.Types;

namespace UselessFrame.NewRuntime.ECS
{
    public class EventManager
    {
        private class SystemHandle
        {
            private Delegate _method;
            private object[] _params;

            public SystemHandle(Delegate method, int paramCount)
            {
                _method = method;
                _params = new object[paramCount];
            }

            public void Invoke(object p)
            {
                _params[0] = p;
                _method.DynamicInvoke(_params);
            }

            public void Invoke(object p1, object p2)
            {
                _params[0] = p1;
                _params[1] = p2;
                _method.DynamicInvoke(_params);
            }
        }

        private Dictionary<Type, List<SystemHandle>> _awakeSystems;
        private Dictionary<Type, List<SystemHandle>> _updateSystems;
        private Dictionary<Type, List<SystemHandle>> _destroySystems;

        public void Initialize()
        {
            _awakeSystems = new Dictionary<Type, List<SystemHandle>>();
            _destroySystems = new Dictionary<Type, List<SystemHandle>>();
            _updateSystems = new Dictionary<Type, List<SystemHandle>>();

            CollectHandle(typeof(IAwakeSystem), typeof(IAwakeSystem<>), typeof(OnAwakeDelegate<>), _awakeSystems);
            CollectHandle(typeof(IDestroySystem), typeof(IDestroySystem<>), typeof(OnDestroyDelegate<>), _destroySystems);
            CollectHandle(typeof(IUpdateSystem), typeof(IUpdateSystem<>), typeof(OnUpdateDelegate<>), _updateSystems);
        }

        private void CollectHandle(Type type1, Type type2, Type awakeDelegateType, Dictionary<Type, List<SystemHandle>> map)
        {
            ITypeCollection types = X.Type.GetCollection(type1);
            foreach (var type in types)
            {
                if (type.IsInterface)
                    continue;

                Type[] interfaces = type.GetInterfaces();
                Type targetF = null;
                foreach (Type f in interfaces)
                {
                    if (!f.IsGenericType)
                        continue;

                    Type genType = f.GetGenericTypeDefinition();
                    if (genType == type2)
                    {
                        targetF = f;
                        break;
                    }
                }

                if (targetF != null)
                {
                    var target = targetF.GetMethods()[0];
                    var tType = targetF.GetGenericArguments()[0];
                    object obj = X.Type.CreateInstance(type);
                    Delegate method = target.CreateDelegate(awakeDelegateType.MakeGenericType(tType), obj);
                    SystemHandle handle = new SystemHandle(method, target.GetParameters().Length);
                    if (!map.TryGetValue(tType, out List<SystemHandle> list))
                    {
                        list = new List<SystemHandle>();
                        map[tType] = list;
                    }
                    list.Add(handle);
                }
            }
        }

        public void TriggerComponentAwake(EntityComponent comp)
        {
            Type type = comp.GetType();
            if (_awakeSystems.TryGetValue(type, out List<SystemHandle> list))
            {
                foreach (SystemHandle system in list)
                {
                    system.Invoke(comp);
                }
            }
        }

        public void TriggerComponentUpdate(EntityComponent oldComp, EntityComponent newComp)
        {
            Type type = oldComp.GetType();
            if (_updateSystems.TryGetValue(type, out List<SystemHandle> list))
            {
                foreach (SystemHandle system in list)
                {
                    system.Invoke(oldComp, newComp);
                }
            }
        }

        public void TriggerComponentDestroy(EntityComponent comp)
        {
            Type type = comp.GetType();
            if (_destroySystems.TryGetValue(type, out List<SystemHandle> list))
            {
                foreach (SystemHandle system in list)
                {
                    system.Invoke(comp);
                }
            }
        }
    }
}
