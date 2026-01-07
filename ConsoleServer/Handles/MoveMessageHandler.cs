
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.ECS;

namespace IMGUITestShare
{
    public class MoveMessageHandler : IMessageHandler<MoveMessage>
    {
        private World _world;

        public void OnInit(World world)
        {
            X.Log.Debug($"MoveMessageHandler OnInit");
            _world = world;
        }

        public void OnMessage(MoveMessage message)
        {
            X.Log.Debug($"MoveMessageHandler OnMessage {message}");
            Scene scene = _world.GetScene(message.Scene);
            if (scene == null)
            {
                Entity srcEntity = _world.FindEntity(message.Entity);

                X.Log.Error($"MoveMessageHandler OnMessage scene not found {message.Scene}, src entity is \n{srcEntity}");
                return;
            }
            Entity entity = scene.FindEntity(message.Entity);
            TransformComponent tfCom = entity.GetComponent<TransformComponent>();
            tfCom.Position.x += message.DirectionX;
            tfCom.Position.y += message.DirectionY;
            tfCom.Update();
        }
    }
}
