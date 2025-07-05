
using System;
using System.Threading;

namespace UselessFrame.NewRuntime.Fiber
{
    public class FiberUtility
    {
        public static ILooper RunLoopSleep1(Action<float> handler, CancellationToken token)
        {
            GameLoop looper = new GameLoop(0, handler, token);
            looper.Start();
            return looper;
        }

        public static ILooper RunLoopSleep0(Action<float> handler, CancellationToken token)
        {
            GameLoop looper = new GameLoop(-1, handler, token);
            looper.Start();
            return looper;
        }

        public static ILooper RunLoopFull(Action<float> handler, CancellationToken token)
        {
            GameLoop looper = new GameLoop(-2, handler, token);
            looper.Start();
            return looper;
        }

        public static ILooper RunLoop30Hz(Action<float> handler, CancellationToken token)
        {
            GameLoop looper = new GameLoop(30, handler, token);
            looper.Start();
            return looper;
        }

        public static ILooper RunLoop60Hz(Action<float> handler, CancellationToken token)
        {
            GameLoop looper = new GameLoop(60, handler, token);
            looper.Start();
            return looper;
        }

        public static ILooper RunLoop144Hz(Action<float> handler, CancellationToken token)
        {
            GameLoop looper = new GameLoop(144, handler, token);
            looper.Start();
            return looper;
        }
    }
}
