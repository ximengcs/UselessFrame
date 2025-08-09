
using System;
using UselessFrame.Runtime.Types;

namespace UselessFrame.NewRuntime
{
    public struct XSetting
    {
        public ITypeFilter TypeFilter;

        public ILogger[] Loggers;

        public Type[] ModuleAttributes;

        public ValueTuple<Type, object>[] Modules;

        public string ArchivePath;

        public string EntranceProcedure;
    }
}
