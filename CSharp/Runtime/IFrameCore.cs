using System;
using UselessFrame.Runtime.Diagnotics;
using UselessFrame.Runtime.Pools;
using UselessFrame.Runtime.Types;

namespace UselessFrame.Runtime
{
    public interface IFrameCore : IModuleDriver
    {
        int Id { get; }

        ITypeSystem TypeSystem { get; }

        ILogSystem Log { get; }

        IPoolSystem Pool { get; }

        void Trigger<T>(object data);

        void Trigger<T>(float data);

        void AddHandler(Type handleType);
    }
}
