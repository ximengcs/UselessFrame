
using QuadTrees;
using System.Collections.Generic;
using UselessFrame.NewRuntime.Entities;
using UselessFrame.NewRuntime.Worlds;

namespace UselessFrame.NewRuntime.Scenes
{
    internal class Scene : Entity
    {
        private World _world;
        private Dictionary<long, Entity> _entitiesRefId;
        private QuadTreeRectF<Entity> _entitiesRefWorld;

        public World World => _world;
    }
}
