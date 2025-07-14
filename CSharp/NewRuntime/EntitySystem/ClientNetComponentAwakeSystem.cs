
namespace UselessFrame.NewRuntime.Entities
{
    public class ClientNetComponentAwakeSystem : IAwakeSystem<ClientNetComponent>, IDestroySystem<ClientNetComponent>
    {
        void IAwakeSystem<ClientNetComponent>.OnAwake(ClientNetComponent comp)
        {
            comp.Entity.Scene.World.Event.AddGlobalAwakeSystem<ClientComponentAwakeSystem>();
        }

        void IDestroySystem<ClientNetComponent>.Destroy(ClientNetComponent comp)
        {
            comp.Entity.Scene.World.Event.RemoveGlobalAwakeSystem<ClientComponentAwakeSystem>();
        }
    }
}
