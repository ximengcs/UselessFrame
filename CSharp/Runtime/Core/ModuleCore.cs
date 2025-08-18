using System;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using UselessFrame.NewRuntime;

namespace UselessFrame.Runtime
{
    internal class ModuleCore : IModuleCore, IManagerUpdater, IManagerDisposable, IManagerInitializer
    {
        private int _id;
        private bool _starting;
        private ModuleDriver _driver;
        private Stopwatch _sw;

        public int Id => _id;

        public ModuleCore(int id)
        {
            _id = id;
            _sw = new Stopwatch();
        }

        public async UniTask Initialize(XSetting setting)
        {
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

        public T Get<T>(int id = 0) where T : IModule
        {
            return (T)Get(typeof(T), id);
        }

        public IModule Get(Type type, int id = 0)
        {
            return _driver.Get(type, id);
        }

        public async UniTask<IModule> Add(Type type, object param)
        {
            _sw.Restart();
            IModule module = await _driver.Add(type, param);
            _sw.Stop();
            X.Log.Debug(FrameLogType.System, $"add module {type.Name}, spent time {_sw.ElapsedMilliseconds}");
            return module;
        }

        public async UniTask Remove(Type type, int id = 0)
        {
            _sw.Restart();
            await _driver.Remove(type, id);
            _sw.Stop();
            X.Log.Debug(FrameLogType.System, $"remove module {type.Name}, spent time {_sw.ElapsedMilliseconds}");
        }
    }
}
