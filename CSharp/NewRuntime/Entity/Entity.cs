
using MemoryPack;
using QuadTrees.QTreeRectF;
using System;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
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

        public long Id => _id;

        public float3 Position;

        public RectangleF Rect => throw new System.NotImplementedException();

        public T AddComponent<T>() where T : IComponent
        {
            Type type = typeof(T);
            IComponent comp = (IComponent)X.Type.CreateInstance(type);
            _components[type] = comp;
            _root.World.Event.TriggerComponentAwake(comp);
            return (T)comp;
        }
    }
}
