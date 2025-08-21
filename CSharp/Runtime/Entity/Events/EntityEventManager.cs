
using Google.Protobuf;
using System;
using System.Collections.Generic;
using UselessFrame.Runtime.Types;

namespace UselessFrame.NewRuntime.ECS
{
    public class EntityEventManager
    {
        private class MethodHandle
        {
            private Delegate _method;
            private object[] _params;

            public object Target { get; }

            public MethodHandle(object target, Delegate method, int paramCount)
            {
                Target = target;
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

        private World _world;
        private Dictionary<Type, List<MethodHandle>> _awakeSystems;
        private Dictionary<Type, List<MethodHandle>> _updateSystems;
        private Dictionary<Type, List<MethodHandle>> _destroySystems;
        private Dictionary<Type, List<MethodHandle>> _messageHandles;

        public void Initialize(World world)
        {
            _world = world;
            _awakeSystems = new Dictionary<Type, List<MethodHandle>>();
            _destroySystems = new Dictionary<Type, List<MethodHandle>>();
            _updateSystems = new Dictionary<Type, List<MethodHandle>>();
            _messageHandles = new Dictionary<Type, List<MethodHandle>>();

            CollectHandle(typeof(IAwakeSystem), typeof(IAwakeSystem<>), typeof(OnAwakeDelegate<>), _awakeSystems);
            CollectHandle(typeof(IDestroySystem), typeof(IDestroySystem<>), typeof(OnDestroyDelegate<>), _destroySystems);
            CollectHandle(typeof(IUpdateSystem), typeof(IUpdateSystem<>), typeof(OnUpdateDelegate<>), _updateSystems);
            CollectHandle(typeof(IMessageHandler), typeof(IMessageHandler<>), typeof(OnMessage<>), _messageHandles);
            InitMessageHandler();
        }

        private void InitMessageHandler()
        {
            Console.WriteLine($"InitMessageHandler {_messageHandles.Count}");
            foreach (var handlerEntry in _messageHandles)
            {
                Console.WriteLine($"InitMessageHandler2 {handlerEntry.Value.Count}");
                foreach (MethodHandle handle in handlerEntry.Value)
                {
                    Console.WriteLine($"InitMessageHandler3 {handle.Target.GetType().Name}");
                    if (handle.Target is IMessageHandler handler)
                    {
                        handler.OnInit(_world);
                    }
                }
            }
            Console.WriteLine($"InitMessageHandler--------");
        }

        private void CollectHandle(Type type1, Type type2, Type awakeDelegateType, Dictionary<Type, List<MethodHandle>> map)
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
                    MethodHandle handle = new MethodHandle(obj, method, target.GetParameters().Length);
                    if (!map.TryGetValue(tType, out List<MethodHandle> list))
                    {
                        list = new List<MethodHandle>();
                        map[tType] = list;
                    }
                    list.Add(handle);
                }
            }
        }

        public void TriggerComponentAwake(EntityComponent comp)
        {
            Type type = comp.GetType();
            if (_awakeSystems.TryGetValue(type, out List<MethodHandle> list))
            {
                foreach (MethodHandle system in list)
                {
                    system.Invoke(comp);
                }
            }
        }

        public void TriggerComponentUpdate(EntityComponent oldComp, EntityComponent newComp)
        {
            Type type = oldComp.GetType();
            if (_updateSystems.TryGetValue(type, out List<MethodHandle> list))
            {
                foreach (MethodHandle system in list)
                {
                    system.Invoke(oldComp, newComp);
                }
            }
        }

        public void TriggerComponentDestroy(EntityComponent comp)
        {
            Type type = comp.GetType();
            if (_destroySystems.TryGetValue(type, out List<MethodHandle> list))
            {
                foreach (MethodHandle system in list)
                {
                    system.Invoke(comp);
                }
            }
        }

        public void TriggerMessage(IMessage message)
        {
            Type type = message.GetType();
            if (_messageHandles.TryGetValue(type, out List<MethodHandle> list))
            {
                foreach (MethodHandle handler in list)
                {
                    handler.Invoke(message);
                }
            }
        }
    }
}
