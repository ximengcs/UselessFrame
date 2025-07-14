using IdGen;
using System;
using System.Collections.Generic;
using UselessFrame.NewRuntime.Scenes;

namespace UselessFrame.NewRuntime.Entities
{
    public partial class Entity
    {
        private long _id;
        private Scene _scene;
        private Entity _parent;
        private Dictionary<long, Entity> _entities;
        private Dictionary<Type, Component> _components;
        private IEntityHelper _helper;

        public Entity Parent => _parent;

        public Scene Scene => _scene;

        public long Id => _id;

        protected Entity()
        {
            _entities = new Dictionary<long, Entity>();
            _components = new Dictionary<Type, Component>();
        }

        internal void Init(IEntityHelper helper)
        {
            _helper = helper;
            OnInit();
        }

        internal void Destroy()
        {
            OnDestroy();
        }

        protected virtual void OnInit()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnAddEntity(Entity entity)
        {

        }

        protected virtual void OnRemoveEntity(Entity entity)
        {

        }

        public T AddEntity<T>() where T : Entity
        {
            Type type = typeof(T);
            T entity = (T)X.Type.CreateInstance(type);
            InitBaseProp(Scene.World.IdGen.CreateId(), this, _scene);
            entity.Init(_helper);
            _entities.Add(entity._id, entity);
            OnAddEntity(entity);
            _helper.OnInit(entity);
            return entity;
        }

        public void RemoveEntity(long id)
        {
            if (_entities.TryGetValue(id, out Entity entity))
            {
                _entities.Remove(id);
                _helper.OnDestroy(entity);
                entity.OnDestroy();
            }
        }

        internal T AddEntity<T>(long id) where T : Entity
        {
            Type type = typeof(T);
            T entity = (T)X.Type.CreateInstance(type);
            InitBaseProp(id, this, _scene);
            entity.Init(_helper);
            _entities.Add(entity._id, entity);
            _helper.OnInit(entity);
            return entity;
        }

        internal void InitBaseProp(long id, Entity parent, Scene scene)
        {
            _id = id;
            _parent = this;
            _scene = scene;
        }

        public T AddComponent<T>() where T : Component
        {
            Type type = typeof(T);
            T comp = (T)X.Type.CreateInstance(type);
            _components[type] = comp;
            comp.OnInit(this);

            _scene.World.Event.TriggerComponentAwake(comp);
            return comp;
        }

        public void RemoveComponent<T>() where T : Component
        {
            Type type = typeof(T);
            if (_components.TryGetValue(type, out Component comp))
            {
                _scene.World.Event.TriggerComponentDestroy(comp);
                comp.OnDestroy();
                _components.Remove(type);
            }
        }
    }
}
