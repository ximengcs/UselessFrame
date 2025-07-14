
using MemoryPack;
using QuadTrees;
using System.Collections.Generic;
using UselessFrame.NewRuntime.Entities;
using UselessFrame.NewRuntime.Worlds;

namespace UselessFrame.NewRuntime.Scenes
{
    public partial class Scene : Entity
    {
        private IEntityHelper _entityHelper;
        private World _world;
        private Dictionary<long, Entity> _entitiesRefId;
        private QuadTreeRectF<TransformComponent> _entitiesRefWorld;

        public World World => _world;

        protected Scene() 
        {

        }
    }
}
