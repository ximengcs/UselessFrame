using System;
using XFrame.Core;
using XFrame.Modules.Pools;
using XFrame.Modules.Archives;
using System.Collections.Generic;
using XFrame.Modules.Diagnotics;

namespace XFrame.Modules.Conditions
{
    internal class ConditionGroupHandle : IConditionGroupHandle
    {
        private bool m_Disposed;
        private bool m_Complete;
        private IDataProvider m_Data;
        private JsonArchive m_Archive;
        private IConditionHelper m_Helper;
        private ConditionSetting m_Setting;
        private List<IConditionHandle> m_AllInfos;
        private Action<IConditionGroupHandle> m_CompleteEvent;
        private Dictionary<int, List<IConditionHandle>> m_NotInfos;
        private ConditionModule m_Module;

        public string Name => m_Setting.Name;

        public bool IsDisposed => m_Disposed;

        public bool Complete => m_Complete;

        public int InstanceId => m_Setting.HelperSetting.UseInstance;

        public ConditionSetting Setting => m_Setting;

        public List<IConditionHandle> AllInfo => m_AllInfos;

        public Dictionary<int, List<IConditionHandle>> NotInfo => m_NotInfos;

        internal ConditionGroupHandle(ConditionModule module, ConditionSetting setting, Action<IConditionGroupHandle> completeCallback = null)
        {
            m_Setting = setting;
            m_Complete = false;
            m_Module = module;
            m_CompleteEvent = completeCallback;
            m_AllInfos = new List<IConditionHandle>();
            m_NotInfos = new Dictionary<int, List<IConditionHandle>>();

            ConditionHelperSetting helperSetting = setting.HelperSetting;
            m_Helper = m_Module.GetOrNewHelper(setting.UseGroupHelper, helperSetting.UseInstance);
            if (helperSetting.UsePersistData)
            {
                InnerEnsureArchive();
                m_Data = m_Archive;
            }
            else
            {
                m_Data = new DataProvider();
            }

            var list = setting.Data.Parser.Value;
            foreach (var node in list)
            {
                ConditionHandle handle = new ConditionHandle(this, node.Value);
                int target = handle.Target;
                ConditionHelperSetting conditionSetting = setting.GetConditionHelperSettting(target);
                CompareInfo compare = m_Module.GetOrNewCompare(target, conditionSetting.UseInstance);
                IDataProvider dataProvider;
                if (conditionSetting.UsePersistData)
                {
                    InnerEnsureArchive();
                    dataProvider = m_Archive.SpwanDataProvider($"condition_group_{m_Setting.Name}_{target}_{conditionSetting.UseInstance}");
                }
                else
                {
                    dataProvider = new DataProvider();
                }

                handle.OnInit(conditionSetting, compare, dataProvider);
                m_AllInfos.Add(handle);
                if (!handle.InnerCheckComplete())
                {
                    if (!m_NotInfos.TryGetValue(handle.Target, out List<IConditionHandle> conds))
                    {
                        conds = new List<IConditionHandle>();
                        m_NotInfos.Add(handle.Target, conds);
                    }
                    conds.Add(handle);
                }
                else
                {
                    ConditionHandle realHandle = handle as ConditionHandle;
                    realHandle.MarkComplete();
                }
            }

            if (m_Helper == null)
                Log.Debug(Log.Condition, $"warning -> condition {Name} helper is null");

            if (m_Helper != null && m_Helper.CheckFinish(this))
            {
                m_Complete = true;
                foreach (IConditionHandle tmp in m_AllInfos)
                {
                    ConditionHandle handle = (ConditionHandle)tmp;
                    handle.MarkComplete();
                }
                m_CompleteEvent?.Invoke(this);
                m_CompleteEvent = null;
            }
            else
            {
                InnerCheckComplete();
            }
        }

        private void InnerEnsureArchive()
        {
            if (m_Archive != null)
                return;
            m_Archive = m_Module.Domain.GetModule<ArchiveModule>().GetOrNew<JsonArchive>($"condition_group_{m_Setting.Name}_{m_Setting.HelperSetting.UseInstance}");
        }

        internal void InnerTrigger(int target, object param)
        {
            if (m_Complete)
                return;
            if (m_NotInfos.TryGetValue(target, out List<IConditionHandle> handles))
            {
                for (int i = handles.Count - 1; i >= 0; i--)
                {
                    ConditionHandle handle = (ConditionHandle)handles[i];
                    if (handle.InnerCheckComplete(param))
                    {
                        handle.MarkComplete();
                        handles.RemoveAt(i);
                    }
                }
                if (handles.Count == 0)
                    m_NotInfos.Remove(target);
                InnerCheckComplete();
            }
        }

        private void InnerCheckComplete()
        {
            if (m_NotInfos.Count == 0)
            {
                m_Complete = true;
                m_Helper.MarkFinish(this);
                m_CompleteEvent?.Invoke(this);
                m_CompleteEvent = null;
            }
        }

        internal void Dispose()
        {
            if (m_Disposed)
                return;
            if (m_Setting.HelperSetting.IsUseInstance)
                References.Release(m_Helper);

            foreach (IConditionHandle h in m_AllInfos)
            {
                ConditionHandle handle = (ConditionHandle)h;
                handle.Dispose();
            }

            m_Disposed = true;
            m_Helper = null;
        }

        public void OnComplete(Action<IConditionGroupHandle> callback)
        {
            if (m_Complete)
                callback?.Invoke(this);
            else
                m_CompleteEvent += callback;
        }

        public bool HasData<T>()
        {
            return m_Data.HasData<T>();
        }

        public bool HasData<T>(string name)
        {
            return m_Data.HasData<T>(name);
        }

        public void SetData<T>(T value)
        {
            m_Data.SetData(value);
        }

        public T GetData<T>()
        {
            return m_Data.GetData<T>();
        }

        public void SetData<T>(string name, T value)
        {
            m_Data.SetData<T>(name, value);
        }

        public T GetData<T>(string name)
        {
            return m_Data.GetData<T>(name);
        }

        public void ClearData()
        {
            m_Data.ClearData();
        }
    }
}
