using System;
using System.Collections.Generic;
using System.Reflection;

namespace XFrame.Modules.Conditions
{
    internal struct CompareDelegateInfo
    {
        public MethodInfo Check;
        public MethodInfo OnEventTrigger;

        public CompareDelegateInfo(MethodInfo check, MethodInfo onEventTrigger)
        {
            Check = check;
            OnEventTrigger = onEventTrigger;
        }
    }

    internal struct CompareInfo
    {
        private CompareDelegateInfo m_DeleInfo;
        private object[] m_CheckParam;
        private object[] m_OnTriggerParam;
        private static Dictionary<Type, CompareDelegateInfo> m_DeleCache;
        public IConditionCompare Inst { get; }
        public CompareDelegateInfo DeleInfo => m_DeleInfo;

        public int Target => Inst.Target;
        public bool Valid => Inst != null;

        public bool CheckFinish(IConditionHandle info)
        {
            return Inst.CheckFinish(info);
        }

        public bool Check(IConditionHandle info, object param)
        {
            m_CheckParam[0] = info;
            m_CheckParam[1] = param;
            return (bool)DeleInfo.Check.Invoke(Inst, m_CheckParam);
        }

        public void OnEventTrigger(object param)
        {
            m_OnTriggerParam[0] = param;
            DeleInfo.OnEventTrigger.Invoke(Inst, m_OnTriggerParam);
        }

        public CompareInfo(IConditionCompare compare)
        {
            Inst = compare;
            Type type = compare.GetType();
            if (m_DeleCache == null)
                m_DeleCache = new Dictionary<Type, CompareDelegateInfo>();

            m_CheckParam = new object[2];
            m_OnTriggerParam = new object[1];
            if (!m_DeleCache.TryGetValue(type, out m_DeleInfo))
            {
                m_DeleInfo = new CompareDelegateInfo();
                Type interfaceType = type.GetInterface(typeof(IConditionCompare<>).FullName);
                Type geneType = interfaceType.GetGenericArguments()[0];
                m_DeleInfo.Check = interfaceType.GetMethod(nameof(Check));
                m_DeleInfo.OnEventTrigger = interfaceType.GetMethod(nameof(OnEventTrigger));
                m_DeleCache.Add(type, DeleInfo);
            }
        }
    }
}
