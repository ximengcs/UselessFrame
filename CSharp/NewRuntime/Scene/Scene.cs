
using QuadTrees;
using System.Collections.Generic;
using UselessFrame.NewRuntime.Entities;

namespace UselessFrame.NewRuntime.Scenes
{
    internal class Scene : Entity
    {
        private Dictionary<long, Entity> _entitiesRefId;
        private QuadTreeRectF<Entity> _entitiesRefWorld;


    }
}
