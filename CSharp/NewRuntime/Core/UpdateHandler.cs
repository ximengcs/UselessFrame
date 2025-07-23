
using Cysharp.Threading.Tasks;
using System;

namespace UselessFrame.Runtime
{
    public class UpdateHandler : IModuleHandler
    {
        private Type _target;

        public Type Target
        {
            get
            {
                if (_target == null)
                    _target = typeof(IUpdater);
                return _target;
            }
        }

        public void Handle(IModule module, float data)
        {
            IUpdater updater = (IUpdater)module;
            updater.OnUpdate(data);
        }
    }
}
