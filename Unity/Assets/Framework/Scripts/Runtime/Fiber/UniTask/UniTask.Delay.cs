
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UselessFrame.NewRuntime.Fiber;

namespace UselessFrame.NewRuntime
{
    public partial struct UniTaskExt
    {
        public static UniTask Delay(float delaySeconds, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (delaySeconds < 0)
            {
                throw new ArgumentOutOfRangeException("Delay does not allow minus delayTimeSpan. delayTimeSpan:" + delaySeconds);
            }

            return new UniTask(DelayPromise.Create(delaySeconds, cancellationToken, out var token), token);
        }


        sealed class DelayPromise : IUniTaskSource, IFiberLoopItem, ITaskPoolNode<DelayPromise>
        {
            static TaskPool<DelayPromise> pool;
            DelayPromise nextNode;
            public ref DelayPromise NextNode => ref nextNode;

            static DelayPromise()
            {
                TaskPool.RegisterSizeGetter(typeof(DelayPromise), () => pool.Size);
            }

            long initialFrame;
            float delayTimeSpan;
            float elapsed;
            IFiber fiber;
            CancellationToken cancellationToken;

            UniTaskCompletionSourceCore<object> core;

            DelayPromise()
            {
            }

            public static IUniTaskSource Create(float delaySeconds, CancellationToken cancellationToken, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                {
                    result = new DelayPromise();
                }

                result.elapsed = 0.0f;
                result.delayTimeSpan = delaySeconds;
                result.cancellationToken = cancellationToken;
                result.fiber = IFiber.Current;
                result.initialFrame = result.fiber.FrameCount;

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

                if (elapsed == 0.0f)
                {
                    if (initialFrame == fiber.FrameCount)
                    {
                        return true;
                    }
                }

                elapsed += fiber.DeltaTime;
                if (elapsed >= delayTimeSpan)
                {
                    core.TrySetResult(null);
                    return false;
                }

                return true;
            }

            bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                core.Reset();
                delayTimeSpan = default;
                elapsed = default;
                cancellationToken = default;
                return pool.TryPush(this);
            }
        }
    }
}
