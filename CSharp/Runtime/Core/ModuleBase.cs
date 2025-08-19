using System.Threading;
using Cysharp.Threading.Tasks;

namespace UselessFrame.Runtime
{
    public abstract class ModuleBase : IModule
    {
        private int _id;
        private bool _start;
        private IModuleCore _core;

        protected CancellationTokenSource _destroyTokenSource;

        public int Id => _id;

        public IModuleCore Core => _core;

        internal void OnModuleInit(IModuleCore core, int id, object param)
        {
            _id = id;
            _start = false;
            _core = core;
            _destroyTokenSource = new CancellationTokenSource();
            OnInit(param);
        }

        protected virtual void OnInit(object param) { }

        internal async UniTask OnModuleStart()
        {
            if (_start)
                return;

            _start = true;
            await OnStart();
        }

        protected virtual async UniTask OnStart() { }

        internal void OnModuleDestroy()
        {
            if (_destroyTokenSource.IsCancellationRequested)
            {
                return;
            }

            _start = false;
            _destroyTokenSource.Cancel();
            OnDestroy();
        }

        protected virtual void OnDestroy() { }

        public static implicit operator bool(ModuleBase module)
        {
            if (module == null)
                return false;

            CancellationTokenSource tokenSource = module._destroyTokenSource;
            if (tokenSource == null || tokenSource.IsCancellationRequested)
                return false;
            return true;
        }
    }
}
