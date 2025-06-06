
using Google.Protobuf;
using UselessFrame.NewRuntime.Router;
using UselessFrame.NewRuntime.Scene;
using UselessFrame.NewRuntime.World;

namespace UselessFrame.NewRuntime
{
    public class WorldManager : IWorldManager
    {
        public ISceneManager Scene => throw new System.NotImplementedException();

        public IRouter Add()
        {
            throw new System.NotImplementedException();
        }

        public void Broadcast(IMessage message)
        {
            throw new System.NotImplementedException();
        }

        public IWorld GetWorld(int worldId)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(IRouter router)
        {
            throw new System.NotImplementedException();
        }
    }
}
