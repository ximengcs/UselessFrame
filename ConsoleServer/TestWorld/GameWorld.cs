
using TestIMGUI.Entities;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.ECS;

namespace TestServer.TestWorld
{
    public class GameWorld
    {
        private World _world;

        public GameWorld()
        {
            _world = X.World.Create(WorldSetting.Server(8888));
            Scene scene = _world.AddEntity<Scene>();
            for (int i = 0; i < 10; i++)
            {
                Entity entity = scene.AddEntity<Entity>();
                entity.GetOrAddComponent<ColorComponent>();
            }
        }
    }
}
