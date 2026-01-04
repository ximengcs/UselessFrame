using System;
using UselessFrame.NewRuntime;
using UselessFrame.Runtime.Collections;
using XFrame.Core;

namespace XFrame.Modules.Conditions
{
    internal partial class ConditionHandle : IConditionHandle
    {
        private int m_Target;
        private ConditionData.Param m_Param;
        private IDataProvider m_Data;
        private ConditionGroupHandle m_Group;
        private Action<IConditionHandle> m_OnComplete;
        private Action<object, object> m_UpdateEvent;
        private bool m_Complete;
        private object m_Value;

        private ConditionHelperSetting m_Setting;
        private CompareInfo m_HandleInfo;

        public IDataProvider Data => m_Data;

        public int Target => m_Target;

        public bool IsComplete => m_Complete;

        public ConditionData.Param Param => m_Param;

        public IConditionGroupHandle Group => m_Group;

        public int InstanceId => m_Setting.UseInstance;

        internal ConditionHandle(ConditionGroupHandle group, ConditionData.Item item)
        {
            m_Group = group;
            m_Target = item.Type;
            m_Param = item.Value;
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
                X.Pool.Release(m_HandleInfo.Inst);
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
                X.Log.Error(FrameLogType.Condition, $"Target {Target} compare is null");
                return false;
            }

            return m_HandleInfo.CheckFinish(this);
        }

        internal bool InnerCheckComplete(object param)
        {
            if (!m_HandleInfo.Valid)
            {
                X.Log.Error(FrameLogType.Condition, $"Target {Target} compare is null");
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
    }
}
