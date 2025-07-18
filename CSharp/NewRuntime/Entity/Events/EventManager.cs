
using System;
using System.Collections.Generic;
using UselessFrame.NewRuntime.Entities;

namespace UselessFrame.NewRuntime.Events
{
    public class EventManager
    {
        private Dictionary<Type, IAwakeSystem> _globalAwakeSystems;
        private Dictionary<Type, List<IAwakeSystem>> _awakeSystems;
        private Dictionary<Type, List<IDestroySystem>> _destroySystems;

        public void Initialize()
        {
            _globalAwakeSystems = new Dictionary<Type, IAwakeSystem>();
            _awakeSystems = new Dictionary<Type, List<IAwakeSystem>>();
            _destroySystems = new Dictionary<Type, List<IDestroySystem>>();
        }

        public void AddGlobalAwakeSystem<T>() where T : IAwakeSystem
        {
            T awakeSys = (T)X.Type.CreateInstance(typeof(T));
            _globalAwakeSystems.Add(typeof(T), awakeSys);
        }

        public void RemoveGlobalAwakeSystem<T>() where T : IAwakeSystem
        {
            _globalAwakeSystems.Remove(typeof(T));
        }

        public void TriggerComponentAwake(Component comp)
        {
            Type type = comp.GetType();
            if (_awakeSystems.TryGetValue(type, out List<IAwakeSystem> list))
            {
                foreach (IAwakeSystem system in list)
                {
                    system.OnAwake(comp);
                }
            }
        }

        public void TriggerComponentUpdate(Component comp)
        {
            Type type = comp.GetType();
            //if (_awakeSystems.TryGetValue(type, out List<IAwakeSystem> list))
            //{
            //    foreach (IAwakeSystem system in list)
            //    {
            //        system.OnAwake(comp);
            //    }
            //}
        }

        public void TriggerComponentDestroy(Component comp)
        {
            Type type = comp.GetType();
            if (_destroySystems.TryGetValue(type, out List<IDestroySystem> list))
            {
                foreach (IDestroySystem system in list)
                {
                    system.Destroy(comp);
                }
            }
        }
    }
}
