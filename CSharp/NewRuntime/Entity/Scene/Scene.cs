using QuadTrees;
using System.Drawing;
using System.Collections.Generic;
using UselessFrame.NewRuntime.Worlds;

namespace UselessFrame.NewRuntime.ECS
{
    public partial class Scene : Entity
    {
        private Dictionary<long, Entity> _entitiesRefId;
        private QuadTreeRectF<TransformComponent> _entitiesRefWorld;

        public World World => (World)Parent;

        protected override void OnInit()
        {
            Scene = this;
            base.OnInit();
            _entitiesRefId = new Dictionary<long, Entity>();
            _entitiesRefWorld = new QuadTreeRectF<TransformComponent>(RectangleF.Empty);
        }

        public Entity FindEntity(long id)
        {
            if (_entitiesRefId.TryGetValue(id, out Entity entity))
            {
                return entity;
            }

            return null;
        }

        internal void RegisterEntity(Entity entity)
        {
            _entitiesRefId.Add(entity.Id, entity);
            TransformComponent tfComp = entity.GetComponent<TransformComponent>();
            if (tfComp == null)
                tfComp = entity.GetOrAddComponent<TransformComponent>();
            _entitiesRefWorld.Add(tfComp);
        }

        internal void UnRegisterEntity(Entity entity)
        {
            _entitiesRefId.Remove(entity.Id);
            _entitiesRefWorld.Remove(entity.GetComponent<TransformComponent>());
        }
    }
}
