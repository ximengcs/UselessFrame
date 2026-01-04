using System.Collections.Generic;
using UselessFrame.Net;

namespace UselessFrame.NewRuntime.ECS
{
    public class WorldManager
    {
        private Dictionary<long, World> _worldList;

        public WorldManager()
        {
            _worldList = new Dictionary<long, World>();
        }

        public World Create(WorldSetting setting)
        {
            IEntityHelper entityHelper = null;
            switch (setting.Type)
            {
                case WorldType.None:
                    {
                        entityHelper = new DefaultEntityHelper();
                        break;
                    }

                case WorldType.Server:
                    {
                        entityHelper = new ServerEntityHelper(setting.Ip.Port, setting.Fiber);
                        break;
                    }

                case WorldType.Client:
                    {
                        IConnection connection = X.Net.Connect(setting.Ip, setting.Fiber);
                        entityHelper = new ClientEntityHelper(connection);
                        break;
                    }
                    
            }
            World world = new World();
            entityHelper.Bind(world);
            world.Init(entityHelper);
            _worldList.Add(world.Id, world);
            return world;
        }

        public void Destroy(World world)
        {
            if (_worldList.ContainsKey(world.Id))
            {
                world._helper.OnDestroyEntity(world);
                _worldList.Remove(world.Id);
                world.Destroy();
            }
        }
    }
}
