
using Google.Protobuf;

namespace UselessFrame.NewRuntime.ECS
{
    public interface IMessageHandler
    {
        void OnInit(World world);
    }

    public interface IMessageHandler<T> : IMessageHandler where T : IMessage
    {
        void OnMessage(T message);
    }

    public delegate void OnMessage<T>(T message);
}
