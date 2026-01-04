
using Google.Protobuf;
using UselessFrame.Net;

namespace UselessFrame.NewRuntime.ECS
{
    internal interface ICombineEntityHelper
    {
        INetNode NetNode { get; }

        void AddHelper(IEntityHelper helper);

        void Trigger(IMessage message);
    }
}
