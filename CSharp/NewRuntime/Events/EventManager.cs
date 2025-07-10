
using System;
using System.Collections.Generic;
using UselessFrame.NewRuntime.Entities;

namespace UselessFrame.NewRuntime.Events
{
    internal class EventManager
    {
        private Dictionary<Type, List<IAwakeSystem>> _awakeSystems;

        public void Initialize()
        {
            _awakeSystems = new Dictionary<Type, List<IAwakeSystem>>();
        }

        public void Trigger(IComponent comp, params object[] objects)
        {

        }

        public void TriggerComponentAwake(IComponent comp)
        {
            Type type = comp.GetType();
            if (_awakeSystems.TryGetValue(type, out List<IAwakeSystem> list))
            {
                foreach (IAwakeSystem system in list)
                {
                    system.Awake(comp);
                }
            }
        }
    }
}
