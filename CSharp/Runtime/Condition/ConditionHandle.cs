using System;
using XFrame.Core;
using XFrame.Modules.Pools;
using XFrame.Modules.Diagnotics;

namespace XFrame.Modules.Conditions
{
    internal partial class ConditionHandle : IConditionHandle
    {
        private int m_Target;
        private UniversalParser m_Param;
        private IDataProvider m_Data;
        private ConditionGroupHandle m_Group;
        private Action<IConditionHandle> m_OnComplete;
        private Action<object, object> m_UpdateEvent;
        private bool m_Complete;
        private object m_Value;

        private ConditionHelperSetting m_Setting;
        private CompareInfo m_HandleInfo;

        public int Target => m_Target;

        public bool IsComplete => m_Complete;

        public UniversalParser Param => m_Param;

        public IConditionGroupHandle Group => m_Group;

        public int InstanceId => m_Setting.UseInstance;

        internal ConditionHandle(ConditionGroupHandle group, PairParser<IntOrHashParser, UniversalParser> parser)
        {
            m_Group = group;
            Pair<IntOrHashParser, UniversalParser> pair = parser;
            m_Target = pair.Key;
            m_Param = pair.Value;
            m_Complete = false;
        }

        internal void OnInit(ConditionHelperSetting setting, CompareInfo helper, IDataProvider dataProvider)
        {
            m_Setting = setting;
            m_HandleInfo = helper;
            m_Data = dataProvider;
        }

        internal void Dispose()
        {
            if (m_Setting.IsUseInstance)
            {
                References.Release(m_HandleInfo.Inst);
                m_HandleInfo = default;
            }
        }

        internal void MarkComplete()
        {
            if (m_Complete)
                return;
            m_Complete = true;
            m_OnComplete?.Invoke(this);
            m_OnComplete = null;
        }

        internal bool InnerCheckComplete()
        {
            if (!m_HandleInfo.Valid)
            {
                Log.Error(Log.Condition, $"Target {Target} compare is null");
                return false;
            }

            return m_HandleInfo.CheckFinish(this);
        }

        internal bool InnerCheckComplete(object param)
        {
            if (!m_HandleInfo.Valid)
            {
                Log.Error(Log.Condition, $"Target {Target} compare is null");
                return false;
            }

            return m_HandleInfo.Check(this, param);
        }

        public void Trigger(object oldValue, object newValue)
        {
            m_Value = newValue;
            m_UpdateEvent?.Invoke(oldValue, newValue);
        }

        public void OnUpdate(Action<object, object> callback)
        {
            if (m_Value != null)
                callback?.Invoke(m_Value, m_Value);
            m_UpdateEvent += callback;
        }

        public void OnComplete(Action<IConditionHandle> callback)
        {
            if (m_Complete)
            {
                callback?.Invoke(this);
            }
            else
            {
                m_OnComplete += callback;
            }
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
            m_Data.SetData(name, value);
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
