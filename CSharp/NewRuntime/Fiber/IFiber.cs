﻿
using Cysharp.Threading.Tasks;
using System.Threading;

namespace UselessFrame.NewRuntime.Fiber
{
    public interface IFiber
    {
        int ThreadId { get; }

        bool IsMain { get; }

        long FrameCount { get; }

        float DeltaTime { get; }

        bool IsCurrent => this == Current;

        SwitchToSynchronizationContextAwaitable Switch();

        void Post(SendOrPostCallback d, object state);

        void Add(IFiberLoopItem loopItem);

        void Dispose();

        static IFiber Current => FiberExtensions.GetCurrentContextFiber();
    }
}
