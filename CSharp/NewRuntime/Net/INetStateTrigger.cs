
namespace UselessFrame.Net
{
    internal interface INetStateTrigger
    {
        void TriggerState(int oldState, int newState);
    }
}
