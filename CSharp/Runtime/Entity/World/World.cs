
using IdGen;
using Google.Protobuf;
using UselessFrame.Net;
using System.Collections.Generic;
using UselessFrame.NewRuntime.Randoms;
using UselessFrame.NewRuntime.Utilities;

namespace UselessFrame.NewRuntime.ECS
{
    public class World : Entity
    {
        private Dictionary<long, Entity> _entities;
        private Dictionary<long, Scene> _scenes;
        private IdGenerator _idGenerator;
        private EntityEventManager _event;
        private TimeRandom _random;

        public IRandom Random => _random;

        public IdGenerator IdGen => _idGenerator;

        public EntityEventManager Event => _event;

        public IReadOnlyCollection<Scene> Scenes => _scenes.Values;

        public INetNode NetNode
        {
            get
            {
                if (_helper is ICombineEntityHelper helper)
                    return helper.NetNode;
                return null;
            }
        }

        protected override void OnInit()
        {
            base.OnInit();
            _entities = new Dictionary<long, Entity>();
            _scenes = new Dictionary<long, Scene>();
            ITimeSource timeSource = new TimeTicksSource();
            _idGenerator = new IdGenerator(0, new IdGeneratorOptions(timeSource: timeSource));
            _random = new TimeRandom(timeSource);
            _event = new EntityEventManager();
            _event.Initialize(this);
        }

        public Entity FindEntity(long entityId)
        {
            if (_entities.TryGetValue(entityId, out Entity entity))
            {
                return entity;
            }
            return null;
        }

        internal void InitId()
        {
            Id = _idGenerator.CreateId();
        }

        public void Trigger(IMessage message)
        {
            if (_helper is ICombineEntityHelper helper)
                helper.Trigger(message);
        }

        public void SetHelper(IEntityHelper helper)
        {
            ICombineEntityHelper canAdd = _helper as ICombineEntityHelper;
            if (canAdd != null)
                canAdd.AddHelper(helper);
        }

        public Scene GetScene(long id)
        {
            if (_scenes.TryGetValue(id, out Scene scene)) return scene;
            return null;
        }

        internal void RegisterEntity(Entity entity)
        {
            if (entity is Scene scene)
            {
                _scenes.Add(scene.Id, scene);
            }
            _entities.Add(entity.Id, entity);
        }

        internal void UnRegisterEntity(Entity entity)
        {
            if (entity is Scene scene)
            {
                _scenes.Remove(scene.Id);
            }
            _entities.Remove(entity.Id);
        }
    }
}
