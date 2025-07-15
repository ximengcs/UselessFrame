using QuadTrees;
using System.Collections.Generic;
using System.Drawing;
using UselessFrame.NewRuntime.Entities;
using UselessFrame.NewRuntime.Worlds;

namespace UselessFrame.NewRuntime.Scenes
{
    public partial class Scene : Entity
    {
        private Dictionary<long, Entity> _entitiesRefId;
        private QuadTreeRectF<TransformComponent> _entitiesRefWorld;

        public World World => (World)Parent;

        public IReadOnlyCollection<Entity> Entities => _entitiesRefId.Values;

        protected override void OnInit()
        {
            Scene = this;
            base.OnInit();
            _entitiesRefId = new Dictionary<long, Entity>();
            _entitiesRefWorld = new QuadTreeRectF<TransformComponent>(RectangleF.Empty);
        }

        protected override void OnAddEntity(Entity entity)
        {
            base.OnAddEntity(entity);
            _entitiesRefId.Add(entity.Id, entity);
            _entitiesRefWorld.Add(entity.GetComponent<TransformComponent>());
        }

        protected override void OnRemoveEntity(Entity entity)
        {
            base.OnRemoveEntity(entity);
            _entitiesRefId.Remove(entity.Id);
            _entitiesRefWorld.Remove(entity.GetComponent<TransformComponent>());
        }
    }
}
