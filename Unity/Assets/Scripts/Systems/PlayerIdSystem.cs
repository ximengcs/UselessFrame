
using UselessFrame.Net;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.ECS;

namespace TestGame
{
    internal class PlayerIdSystem : IAwakeSystem<IdComponent>
    {
        private static PlayerEntity _player;

        public static PlayerEntity Player => _player;

        public void OnAwake(IdComponent comp)
        {
            X.Log.Debug($"PlayerIdSystem OnAwake {comp.Id}");
            if (comp.Entity is PlayerEntity player)
            {
                if (player.World.NetNode is IConnection connection && connection.Id == comp.Id)
                {
                    _player = player;
                }
            }
        }
    }
}
