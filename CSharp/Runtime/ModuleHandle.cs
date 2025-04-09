
using System.Collections.Generic;

namespace UselessFrame.Runtime
{
    internal struct ModuleHandle
    {
        public IModuleHandler Handler;
        public List<IModule> Modules;

        public ModuleHandle(IModuleHandler handler, List<IModule> modules)
        {
            Handler = handler;
            Modules = modules;
        }
    }
}
