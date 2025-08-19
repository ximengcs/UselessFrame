
namespace UselessFrame.NewRuntime.Fiber
{
    public interface ILooper
    {
        LoopState State { get; }

        void Start();

        void Stop();

        void Pause();

        void Continue();

        void Flush(bool flush);
    }
}
