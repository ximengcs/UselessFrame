
using System;
using System.Diagnostics;
using System.Threading;

namespace UselessFrame.NewRuntime.Fiber
{
    public class FiberUtility
    {
        public static float TimestampToTicks = TimeSpan.TicksPerSecond / (float)Stopwatch.Frequency;

        public static ILooper RunLoopSleep1(Action<float> handler, CancellationToken token)
        {
            GameLoop looper = new GameLoop(handler, token);
            looper.Start();
            return looper;
        }

        public static ILooper RunLoopSleep0(Action<float> handler, CancellationToken token)
        {
            PirceseGameLoop looper = new PirceseGameLoop(-1, handler, token);
            looper.Start();
            return looper;
        }

        public static ILooper RunLoopFull(Action<float> handler, CancellationToken token)
        {
            PirceseGameLoop looper = new PirceseGameLoop(-2, handler, token);
            looper.Start();
            return looper;
        }

        public static ILooper RunLoop30Hz(Action<float> handler, CancellationToken token)
        {
            PirceseGameLoop looper = new PirceseGameLoop(30, handler, token);
            looper.Start();
            return looper;
        }

        public static ILooper RunLoop60Hz(Action<float> handler, CancellationToken token)
        {
            PirceseGameLoop looper = new PirceseGameLoop(60, handler, token);
            looper.Start();
            return looper;
        }

        public static ILooper RunLoop144Hz(Action<float> handler, CancellationToken token)
        {
            PirceseGameLoop looper = new PirceseGameLoop(144, handler, token);
            looper.Start();
            return looper;
        }
    }
}
