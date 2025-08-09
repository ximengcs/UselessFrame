
using System;

namespace UselessFrame.NewRuntime
{
    public interface ILogManager
    {
        void AddLogger<T>() where T : ILogger;
        T GetLogger<T>() where T : ILogger;
        void Debug(params object[] content);
        void Warning(params object[] content);
        void Error(params object[] content);
        void Fatal(params object[] content);
        void Exception(Exception e);
    }
}
