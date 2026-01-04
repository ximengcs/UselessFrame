using System;
using System.Reflection;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UselessFrame.Runtime.Collections;
using ModuleList = UselessFrame.Runtime.Collections.XList<UselessFrame.Runtime.IModule>;
using UselessFrame.NewRuntime;

namespace UselessFrame.Runtime
{
    internal class ModuleDriver : IModuleDriver
    {
        private bool _start;
        private IModuleCore _core;
        private ModuleCollection _modules;
        private Dictionary<Type, ModuleHandle> m_ModulesWithEvents;

        public IModuleCore Core => _core;

        public ModuleDriver(IModuleCore core)
        {
            _core = core;
            _modules = new ModuleCollection();
            m_ModulesWithEvents = new Dictionary<Type, ModuleHandle>();
        }

        public void Trigger(Type handlerType, object data)
        {
            if (m_ModulesWithEvents.TryGetValue(handlerType, out ModuleHandle handle))
            {
                foreach (IModule module in handle.Modules)
                {
                    handle.Handler.Handle(module, data);
                }
            }
        }

        public void Trigger(Type handlerType, float data)
        {
            if (m_ModulesWithEvents.TryGetValue(handlerType, out ModuleHandle handle))
            {
                foreach (IModule module in handle.Modules)
                {
                    handle.Handler.Handle(module, data);
                }
            }
        }

        public void AddHandle(IModuleHandler handler)
        {
            Type handleType = handler.Target;
            if (!m_ModulesWithEvents.ContainsKey(handleType))
            {
                ModuleHandle handle = new ModuleHandle(handler, new ModuleList());
                m_ModulesWithEvents[handleType] = handle;
            }
        }

        public IModule Get(Type type, int id)
        {
            return _modules.Get(type, id);
        }

        public async UniTask<IModule> Add(Type type, object param)
        {
            int id = default;
            Attribute attr = type.GetCustomAttribute(typeof(ModuleAttribute));
            if (attr != null)
            {
                ModuleAttribute mAttr = (ModuleAttribute)attr;
                id = mAttr.Id;
            }

            ModuleBase module = _modules.Add(type, id);
            if (module != null)
            {
                X.Log.Debug(FrameLogType.System, $"start add module -> {module.GetType().Name}");
                module.OnModuleInit(_core, id, param);

                foreach (var entry in m_ModulesWithEvents)
                {
                    if (entry.Key.IsAssignableFrom(type))
                    {
                        entry.Value.Modules.Add(module);
                    }
                }
                X.Log.Debug(FrameLogType.System, $"add module complete -> {module.GetType().Name}");

                if (_start)
                    await module.OnModuleStart();
            }

            return module;
        }

        public void Remove(Type type, int id)
        {
            ModuleBase module = _modules.Remove(type, id);
            if (module)
            {
                X.Log.Debug(FrameLogType.System, $"start destroy module -> {module.GetType().Name}");
                module.OnModuleDestroy();
                X.Log.Debug(FrameLogType.System, $"destroy module complete -> {module.GetType().Name}");
                foreach (var entry in m_ModulesWithEvents)
                {
                    if (entry.Key.IsAssignableFrom(type))
                    {
                        entry.Value.Modules.Remove(module);
                    }
                }
            }
        }

        public async UniTask Start()
        {
            foreach (ModuleBase module in _modules)
            {
                X.Log.Debug(FrameLogType.System, $"start run module -> {module.GetType().Name}");
                await module.OnModuleStart();
                X.Log.Debug(FrameLogType.System, $"run module complete -> {module.GetType().Name}");
            }
            _start = true;
        }

        public void Destroy()
        {
            IEnumerator<ModuleBase> it = _modules.GetEnumerator(EnumeratorType.Front);
            while (it.MoveNext())
            {
                ModuleBase module = it.Current;
                X.Log.Debug(FrameLogType.System, $"start destroy module -> {module.GetType().Name}");
                module.OnModuleDestroy();
                X.Log.Debug(FrameLogType.System, $"destroy module complete -> {module.GetType().Name}");
            }
        }
    }
}
