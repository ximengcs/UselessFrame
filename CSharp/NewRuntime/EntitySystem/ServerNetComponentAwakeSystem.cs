
namespace UselessFrame.NewRuntime.Entities
{
    public class ServerNetComponentAwakeSystem : IAwakeSystem<ServerNetComponent>, IDestroySystem<ServerNetComponent>
    {
        void IAwakeSystem<ServerNetComponent>.OnAwake(ServerNetComponent comp)
        {
            comp.Entity.Scene.World.Event.AddGlobalAwakeSystem<ServerComponentAwakeSystem>();
        }

        void IDestroySystem<ServerNetComponent>.Destroy(ServerNetComponent comp)
        {
            comp.Entity.Scene.World.Event.RemoveGlobalAwakeSystem<ServerComponentAwakeSystem>();
        }
    }
}
