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

        public IReadOnlyCollection<Component> Components => _components.Values;

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
            InitEntity(entity, Scene.World.IdGen.CreateId(), this, _scene);
            return entity;
        }

        public void RemoveEntity(long id)
        {
            if (_entities.TryGetValue(id, out Entity entity))
            {
                _entities.Remove(id);
                _helper.OnDestroyEntity(entity);
                entity.OnDestroy();
            }
        }

        internal T AddEntity<T>(long id) where T : Entity
        {
            return (T)AddEntity(typeof(T), id);
        }

        internal Entity AddEntity(Type type, long id)
        {
            Entity entity = (Entity)X.Type.CreateInstance(type);
            InitEntity(entity, id, this, _scene);
            return entity;
        }

        internal void InitEntity(Entity entity, long id, Entity parent, Scene scene)
        {
            entity._id = id;
            entity._parent = this;
            entity._scene = scene;

            entity.Init(_helper);
            _entities.Add(entity._id, entity);
            OnAddEntity(entity);
            _helper.OnCreateEntity(entity);
        }

        internal void InitEntity(long id, IEntityHelper helper)
        {
            _id = id;
            _parent = null;
            _scene = null;

            Init(_helper);
            _helper.OnCreateEntity(this);
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

        public T GetComponent<T>() where T : Component
        {
            Type type = typeof(T);
            if (_components.TryGetValue(type, out Component comp))
                return (T)comp;
            return null;
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
