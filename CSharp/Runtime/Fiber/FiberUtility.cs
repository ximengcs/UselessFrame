
using System;
using System.Diagnostics;
using System.Threading;

namespace UselessFrame.NewRuntime.Fiber
{
    public class FiberUtility
    {
        public static float TimestampToTicks = TimeSpan.TicksPerSecond / (float)Stopwatch.Frequency;

        public static void RunLoopSleep1(Action<float> handler, CancellationToken token, out ILooper looper)
        {
            looper = new GameLoop(handler, token);
            looper.Start();
        }

        public static void RunLoopSleep0(Action<float> handler, CancellationToken token, out ILooper looper)
        {
            looper = new PirceseGameLoop(-1, handler, token);
            looper.Start();
        }

        public static void RunLoopFull(Action<float> handler, CancellationToken token, out ILooper looper)
        {
            looper = new PirceseGameLoop(-2, handler, token);
            looper.Start();
        }

        public static void RunLoop30Hz(Action<float> handler, CancellationToken token, out ILooper looper)
        {
            looper = new PirceseGameLoop(30, handler, token);
            looper.Start();
        }

        public static void RunLoop60Hz(Action<float> handler, CancellationToken token, out ILooper looper)
        {
            looper = new PirceseGameLoop(60, handler, token);
            looper.Start();
        }

        public static void RunLoop144Hz(Action<float> handler, CancellationToken token, out ILooper looper)
        {
            looper = new PirceseGameLoop(144, handler, token);
            looper.Start();
        }
    }
}
