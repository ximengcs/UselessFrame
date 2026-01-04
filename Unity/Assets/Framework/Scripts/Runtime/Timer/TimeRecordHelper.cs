using Cysharp.Threading.Tasks;
using System;
using UselessFrame.NewRuntime.Timers;
using UselessFrame.Runtime.Pools;

namespace UselessFrame.Runtime.Timer
{
    [PoolHelper(typeof(ITimeRecord))]
    internal class TimeRecordHelper : IPoolHelper
    {
        int IPoolHelper.CacheCount => 128;

        IPoolObject IPoolHelper.Factory(Type type, int poolKey)
        {
            return new TimeRecord();
        }

        async UniTask<IPoolObject> IPoolHelper.FactoryAsync(Type type, int poolKey)
        {
            return new TimeRecord();
        }

        void IPoolHelper.OnObjectCreate(IPoolObject obj)
        {
        }

        void IPoolHelper.OnObjectDestroy(IPoolObject obj)
        {
        }

        void IPoolHelper.OnObjectRelease(IPoolObject obj)
        {
        }

        void IPoolHelper.OnObjectRequest(IPoolObject obj)
        {
        }
    }
}
