
using System.Text;
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
            Entity entity = _world.FindEntity(message.Entity);
            TransformComponent tfCom = entity.GetComponent<TransformComponent>();
            if (tfCom != null)
            {
                tfCom.Position.x += message.DirectionX;
                tfCom.Position.y += message.DirectionY;
                tfCom.Update();
            }
            else
            {
                StringBuilder s = new StringBuilder();
                foreach (EntityComponent component in entity.Components)
                {
                    s.Append($" {component.GetType().Name} ");
                }
                X.Log.Error($"can not move {entity.GetType().Name} {entity.Id} ({entity.GetHashCode()}), no transform component, all components : {s.ToString()}");
            }
        }
    }
}
