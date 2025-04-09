using System;
using UselessFrame.Runtime.Types;

namespace UselessFrame.Runtime
{
    public interface IFrameCore : IModuleDriver
    {
        int Id { get; }

        ITypeSystem TypeSystem { get; }

        void Trigger<T>(object data);

        void Trigger<T>(float data);

        void AddHandler(Type handleType);
    }
}
