
using System.Net;

namespace UselessFrame.Net
{
    internal interface INetStateTrigger
    {
        string GetDebugPrefix<T>(NetFsmState<T> state) where T : INetStateTrigger;

        void TriggerState(int newState);
    }
}
