using System.Threading;
using Cysharp.Threading.Tasks;
using UselessFrame.Runtime.Diagnotics;

namespace UselessFrame.Runtime
{
    public abstract class ModuleBase : IModule
    {
        private int _id;
        private bool _start;
        private ModuleDriver _driver;

        protected CancellationTokenSource _destroyTokenSource;

        public int Id => _id;

        public IModuleDriver Driver => _driver;

        internal void OnInit(ModuleDriver driver, int id, object param)
        {
            Log.Debug($"{GetType().Name} OnInit");
            _id = id;
            _start = false;
            _driver = driver;
            _destroyTokenSource = new CancellationTokenSource();
            OnInit(param);
        }

        public virtual void OnInit(object param) { }

        internal async UniTask OnModuleStart()
        {
            if (_start)
                return;

            Log.Debug($"{GetType().Name} OnStart");
            _start = true;
            await OnStart();
        }

        protected virtual async UniTask OnStart() { }

        internal async UniTask OnModuleDestroy()
        {
            Log.Debug($"{GetType().Name} OnDestroy");

            if (_destroyTokenSource.IsCancellationRequested)
            {
                Log.Debug($"module already destroy {GetType().Name}");
                return;
            }

            _start = false;
            _destroyTokenSource.Cancel();
            await OnDestroy();
        }

        protected virtual async UniTask OnDestroy() { }

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
