using System;
using Cysharp.Threading.Tasks;
using UselessFrame.Runtime.Types;
using UselessFrame.Runtime.Configs;

namespace UselessFrame.Runtime
{
    internal class FrameCore : IFrameCore
    {
        private int _id;
        private bool _starting;
        private TypeSystem _typeSystem;
        private ModuleDriver _driver;

        public ITypeSystem TypeSystem => _typeSystem;

        public int Id => _id;

        public FrameCore(int id, FrameConfig config)
        {
            _id = id;
            _typeSystem = new TypeSystem(config.TypeFilter);
            _driver = new ModuleDriver(this);
        }

        public void Trigger<T>(object data)
        {
            _driver.Trigger(typeof(T), data);
        }

        public void Trigger<T>(float data)
        {
            _driver.Trigger(typeof(T), data);
        }

        public void AddHandler(Type handleType)
        {
            IModuleHandler handler = (IModuleHandler)TypeSystem.CreateInstance(handleType);
            _driver.AddHandle(handler);
        }

        public async UniTask Start()
        {
            if (_starting)
                return;
            _starting = true;
            await _driver.Start();
        }

        public async UniTask Destroy()
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
