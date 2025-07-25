
using System;
using UselessFrame.Runtime.Types;

namespace UselessFrame.NewRuntime
{
    public struct XSetting
    {
        public ITypeFilter TypeFilter;

        public Type[] Loggers;

        public Type[] ModuleAttributes;

        public object[] ModuleParams;
    }
}
