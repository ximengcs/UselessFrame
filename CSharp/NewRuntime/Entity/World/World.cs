﻿
using IdGen;
using System.Collections.Generic;
using UselessFrame.NewRuntime.Utilities;

namespace UselessFrame.NewRuntime.ECS
{
    public class World : Entity
    {
        private Dictionary<long, Scene> _scenes;
        private IdGenerator _idGenerator;
        private EventManager _event;

        public IdGenerator IdGen => _idGenerator;

        public EventManager Event => _event;

        public IReadOnlyCollection<Scene> Scenes => _scenes.Values;

        protected override void OnInit()
        {
            base.OnInit();
            _scenes = new Dictionary<long, Scene>();
            _idGenerator = new IdGenerator(0, new IdGeneratorOptions(timeSource: new TimeTicksSource()));
            _event = new EventManager();
            _event.Initialize();
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

        protected override void OnAddEntity(Entity entity)
        {
            base.OnAddEntity(entity);
            if (entity is Scene scene)
            {
                _scenes.Add(scene.Id, scene);
            }
        }

        protected override void OnRemoveEntity(Entity entity)
        {
            base.OnRemoveEntity(entity);
            if (entity is Scene scene)
            {
                _scenes.Remove(scene.Id);
            }
        }
    }
}
