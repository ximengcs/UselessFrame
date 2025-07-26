
using UselessFrame.Runtime.Pools;
using UselessFrame.Runtime.Timer;

namespace UselessFrame.NewRuntime.Timers
{
    public interface ITimeRecord : IPoolObject
    {
        void SetUpdater(ICanGetTime updater);
        void Record(float cd);
        void Record(int key, float cd);
        void Reset(int key);
        void Reset();
        bool Check(int key, bool reset = false);
        bool Check(bool reset = false);
        float CheckTime(int key);
        float CheckTime();
    }
}
