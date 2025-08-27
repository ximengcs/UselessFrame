
using System;
using System.Text;
using System.Collections.Generic;

namespace UselessFrame.NewRuntime.ECS
{
    public partial class Entity
    {
        private long _id;
        private Scene _scene;
        private Entity _parent;
        private Dictionary<long, Entity> _entities;
        private Dictionary<Type, List<Entity>> _entitiesByType;
        private Dictionary<Type, EntityComponent> _components;
        private bool _disposed;
        internal IEntityHelper _helper;

        public IReadOnlyCollection<EntityComponent> Components => _components.Values;

        public Entity Parent => _parent;

        public IReadOnlyCollection<Entity> Entities => _entities.Values;

        public bool IsDisposed => _disposed;

        public World World => _helper.World;

        public Scene Scene
        {
            get => _scene;
            internal set => _scene = value;
        }

        public long Id
        {
            get => _id;
            internal set => _id = value;
        }

        protected Entity()
        {
            _disposed = false;
            _entities = new Dictionary<long, Entity>();
            _components = new Dictionary<Type, EntityComponent>();
            _entitiesByType = new Dictionary<Type, List<Entity>>();
        }

        internal void Init(IEntityHelper helper)
        {
            _helper = helper;
            OnInit();
            _helper.OnCreateEntity(this);
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
            if (_disposed) return;
            _disposed = true;

            List<EntityComponent> components = new List<EntityComponent>(_components.Values);
            foreach (EntityComponent compEntry in components)
            {
                DestoryComponent(compEntry);
            }
            _components = null;
        }

        protected virtual void OnAddEntity(Entity entity)
        {
            _scene?.RegisterEntity(entity);
            World.RegisterEntity(entity);
        }

        protected virtual void OnRemoveEntity(Entity entity)
        {
            _scene?.UnRegisterEntity(entity);
            World.UnRegisterEntity(entity);
        }

        public Entity AddEntity(Type type)
        {
            Entity entity = (Entity)X.Type.CreateInstance(type);
            InitEntity(entity, this.IdGen().CreateId(), this, _scene);
            return entity;
        }

        public T AddEntity<T>() where T : Entity
        {
            return (T)AddEntity(typeof(T));
        }

        public Entity GetEntity(long id)
        {
            if (_entities.TryGetValue(id, out Entity entity))
                return entity;
            return null;
        }

        public T GetEntity<T>() where T : Entity
        {
            if (_entitiesByType.TryGetValue(typeof(T), out List<Entity> entityList))
            {
                if (entityList.Count > 0)
                    return (T)entityList[0];
            }
            return default;
        }

        public bool HasEntity<T>() where T : Entity
        {
            if (_entitiesByType.TryGetValue(typeof(T), out List<Entity> entityList))
            {
                return entityList.Count > 0;
            }
            return false;
        }

        public void RemoveEntity(long id)
        {
            if (_entities.TryGetValue(id, out Entity entity))
            {
                _entities.Remove(id);
                if (_entitiesByType.TryGetValue(entity.GetType(), out List<Entity> entityList))
                    entityList.Remove(entity);
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
            if (entity._scene == null)
            {
                if (entity is Scene entityScene)
                {
                    entity._scene = entityScene;
                }
            }

            entity.Init(_helper);
            _entities.Add(entity._id, entity);

            Type entityType = entity.GetType();
            if (!_entitiesByType.TryGetValue(entityType, out List<Entity> entityList))
            {
                entityList = new List<Entity>();
                _entitiesByType.Add(entityType, entityList);
            }
            entityList.Add(entity);

            OnAddEntity(entity);
        }

        internal void AddOrUpdateComponent(EntityComponent newComp)
        {
            Console.WriteLine($"AddOrUpdateComponent {newComp.GetType().Name}");
            Type type = newComp.GetType();
            if (!_components.TryGetValue(type, out EntityComponent comp))
            {
                InitComponent(type, newComp);
            }
            else
            {
                UpdateComponent(newComp);
            }
        }

        public void AttachComponent(EntityComponent comp)
        {
            Type type = comp.GetType();
            if (!_components.ContainsKey(type))
            {
                InitComponent(type, comp);
            }
        }

        public void UnAttachComponent(EntityComponent comp)
        {
            Type type = comp.GetType();
            if (_components.ContainsKey(type))
            {
                DestoryComponent(comp);
                _components.Remove(type);
            }
        }

        public EntityComponent GetOrAddComponent(Type type)
        {
            Console.WriteLine($"AddOrUpdateComponent2 {type.Name}");
            EntityComponent comp = GetComponent(type);
            if (comp != null) return comp;

            comp = (EntityComponent)X.Type.CreateInstance(type);
            InitComponent(type, comp);
            return comp;
        }

        private EntityEventManager Event => World.Event;

        public void UpdateComponent(EntityComponent newComp)
        {
            Console.WriteLine("UpdateComponent");
            Type type = newComp.GetType();
            if (_components.TryGetValue(type, out EntityComponent comp))
            {
                newComp.OnInit(this);
                _helper.OnUpdateComponent(newComp);
                Event.TriggerComponentUpdate(comp, newComp);
            }
            else
            {
                X.Log.Error($"component is null {type.Name}");
            }
        }

        private void InitComponent(Type type, EntityComponent comp)
        {
            _components[type] = comp;
            comp.OnInit(this);
            Event.TriggerComponentAwake(comp);
            _helper.OnCreateComponent(comp);
        }

        public T GetOrAddComponent<T>() where T : EntityComponent
        {
            return (T)GetOrAddComponent(typeof(T));
        }

        public EntityComponent GetComponent(Type type)
        {
            if (_components.TryGetValue(type, out EntityComponent comp))
                return comp;
            return null;
        }

        public T GetComponent<T>() where T : EntityComponent
        {
            Type type = typeof(T);
            if (_components.TryGetValue(type, out EntityComponent comp))
                return (T)comp;

            foreach (var comEntry in _components)
            {
                if (type.IsAssignableFrom(comEntry.Key))
                    return (T)comEntry.Value;
            }

            return null;
        }

        public void RemoveComponent(EntityComponent comp)
        {
            RemoveComponent(comp.GetType());
        }

        public void RemoveComponent(Type type)
        {
            if (type.IsCoreComponent())
                return;
            if (_components.TryGetValue(type, out EntityComponent comp))
            {
                DestoryComponent(comp);
                _components.Remove(type);
            }
        }

        private void DestoryComponent(EntityComponent comp)
        {
            _helper.OnDestroyComponent(comp);
            Event.TriggerComponentDestroy(comp);
            comp.OnDestroy();
        }

        public void RemoveComponent<T>() where T : EntityComponent
        {
            RemoveComponent(typeof(T));
        }

        public override string ToString()
        {
            return ToString(string.Empty);
        }

        private string ToString(string prefix)
        {
            prefix += "---";
            StringBuilder sb = new StringBuilder();
            sb.Append($"{prefix}[E][{GetType().Name}][{_id}]\n");
            foreach (var comp in _components)
            {
                sb.Append($"---{prefix}[C][{comp.Key.Name}]\n");
            }
            foreach (var child in _entities)
            {
                sb.Append($"{child.Value.ToString(prefix)}");
            }
            return sb.ToString();
        }
    }
}
