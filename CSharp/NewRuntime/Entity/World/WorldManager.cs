using System.Collections.Generic;
using UselessFrame.NewRuntime.Entities;

namespace UselessFrame.NewRuntime.Worlds
{
    public class WorldManager
    {
        private List<World> _worldList;

        public WorldManager()
        {
            _worldList = new List<World>();
        }

        public World Create(IEntityHelper helper)
        {
            World world = new World();
            _worldList.Add(world);
            world.Init(helper);
            return world;
        }

        public void Destroy(World world)
        {
            if (_worldList.Contains(world))
            {
                _worldList.Remove(world);
                world.Destroy();
            }
        }
    }
}
