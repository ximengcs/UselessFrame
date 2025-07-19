using System.Collections.Generic;

namespace UselessFrame.NewRuntime.ECS
{
    public class WorldManager
    {
        private IWorldHelper _helper;
        private Dictionary<long, World> _worldList;

        public IWorldHelper Helper => _helper;

        public WorldManager()
        {
            _worldList = new Dictionary<long, World>();
        }

        public void SetHelper(IWorldHelper helper)
        {
            _helper = helper;
            _helper.OnInit();
        }

        public World Create()
        {
            IEntityHelper entityHelper = _helper.CreateHelper();
            World world = new World();
            world.Init(entityHelper);
            entityHelper.Bind(world);
            _worldList.Add(world.Id, world);
            return world;
        }

        public void Destroy(World world)
        {
            if (_worldList.ContainsKey(world.Id))
            {
                _worldList.Remove(world.Id);
                world.Destroy();
            }
        }
    }
}
