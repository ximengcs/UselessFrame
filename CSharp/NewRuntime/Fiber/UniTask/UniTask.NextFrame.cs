
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UselessFrame.NewRuntime.Fiber;

namespace UselessFrame.NewRuntime
{
    public partial struct UniTaskExt
    {
        public static Cysharp.Threading.Tasks.UniTask NextFrame(CancellationToken cancellationToken)
        {
            return new Cysharp.Threading.Tasks.UniTask(NextFramePromise.Create(cancellationToken, out var token), token);
        }

        sealed class NextFramePromise : IUniTaskSource, IFiberLoopItem, ITaskPoolNode<NextFramePromise>
        {
            static TaskPool<NextFramePromise> pool;
            NextFramePromise nextNode;
            public ref NextFramePromise NextNode => ref nextNode;

            static NextFramePromise()
            {
                TaskPool.RegisterSizeGetter(typeof(NextFramePromise), () => pool.Size);
            }

            long frameCount;
            IFiber fiber;
            CancellationToken cancellationToken;
            UniTaskCompletionSourceCore<AsyncUnit> core;

            NextFramePromise()
            {
            }

            public static IUniTaskSource Create(CancellationToken cancellationToken, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                {
                    result = new NextFramePromise();
                }

                result.fiber = IFiber.Current;
                result.frameCount = result.fiber.FrameCount;
                result.cancellationToken = cancellationToken;

                TaskTracker.TrackActiveTask(result, 3);

                result.fiber.Add(result);

                token = result.core.Version;
                return result;
            }

            public void GetResult(short token)
            {
                try
                {
                    core.GetResult(token);
                }
                finally
                {
                    TryReturn();
                }
            }

            public UniTaskStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            public bool MoveNext()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    core.TrySetCanceled(cancellationToken);
                    return false;
                }

                if (frameCount == fiber.FrameCount)
                {
                    return true;
                }

                core.TrySetResult(AsyncUnit.Default);
                return false;
            }

            bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                core.Reset();
                cancellationToken = default;
                return pool.TryPush(this);
            }
        }
    }
}
