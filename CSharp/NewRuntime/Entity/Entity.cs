
using MemoryPack;
using QuadTrees.QTreeRectF;
using System;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using UselessFrame.NewRuntime.Events;
using UselessFrame.NewRuntime.Scenes;

namespace UselessFrame.NewRuntime.Entities
{
    [MemoryPackable]
    public partial class Entity : IRectFQuadStorable
    {
        private long _id;
        private Scene _root;
        private Entity _parent;
        private Dictionary<long, Entity> _entities;
        private Dictionary<Type, IComponent> _components;
        private Dictionary<int, List<IComponent>> _componentsListType;

        public long Id => _id;

        public float3 Position;

        public RectangleF Rect => throw new System.NotImplementedException();

        public Entity()
        {
            _componentsListType = new Dictionary<int, List<IComponent>>();
        }

        public T AddComponent<T>() where T : IComponent
        {
            Type type = typeof(T);
            IComponent comp = (IComponent)X.Type.CreateInstance(type);
            _components[type] = comp;

            _root.World.Event.TriggerComponentAwake(comp);
            TriggerEvent(XEventType.AddEntityComponent, comp);
            return (T)comp;
        }

        private void TriggerEvent(int eventType, params object[] objects)
        {
            if (_componentsListType.TryGetValue(eventType, out List<IComponent> components))
            {
                foreach (IComponent comp in components)
                {
                    _root.World.Event.Trigger(comp, objects);
                }
            }
        }
    }
}
