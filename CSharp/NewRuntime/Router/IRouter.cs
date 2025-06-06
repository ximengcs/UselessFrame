
using Google.Protobuf;

namespace UselessFrame.NewRuntime.Router
{
    public interface IRouter
    {
        void Broadcast(IMessage message);

        IRouter Add();

        void Remove(IRouter router);
    }
}
