using System;
using Cysharp.Threading.Tasks;
using UselessFrame.NewRuntime;

namespace UselessFrame.Runtime
{
    internal class ModuleCore : IModuleCore, IManagerUpdater, IManagerDisposable
    {
        private int _id;
        private bool _starting;
        private ModuleDriver _driver;

        public int Id => _id;

        public ModuleCore(int id)
        {
            _id = id;
            _driver = new ModuleDriver(this);
        }

        public void Update(float deltaTime)
        {
            Trigger<IModuleUpdater>(deltaTime);
        }

        public void Trigger<T>(object data)
        {
            _driver.Trigger(typeof(T), data);
        }

        private void Trigger<T>(float data)
        {
            _driver.Trigger(typeof(T), data);
        }

        public void AddHandler<T>() where T : IModuleHandler
        {
            IModuleHandler handler = (IModuleHandler)X.Type.CreateInstance(typeof(T));
            _driver.AddHandle(handler);
        }

        public async UniTask Start()
        {
            if (_starting)
                return;
            _starting = true;
            await _driver.Start();
        }

        public async UniTask Dispose()
        {
            if (!_starting)
                return;
            _starting = false;
            await _driver.Destroy();
        }

        public IModule GetModule(Type type, int id = 0)
        {
            return _driver.GetModule(type, id);
        }

        public UniTask<IModule> AddModule(Type type, object param)
        {
            return _driver.AddModule(type, param);
        }

        public UniTask RemoveModule(Type type, int id = 0)
        {
            return _driver.RemoveModule(type, id);
        }
    }
}
