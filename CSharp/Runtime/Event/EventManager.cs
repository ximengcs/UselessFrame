using System.Collections.Generic;

namespace UselessFrame.NewRuntime.Events
{
    /// <inheritdoc/>
    public class EventManager : IManagerInitializer, IEventManager
    {
        private List<EventDispatcher> m_List;

        public void Initialize(XSetting setting)
        {
            m_List = new List<EventDispatcher>();
        }

        /// <inheritdoc/>
        public void OnUpdate(double escapeTime)
        {
            for (int i = m_List.Count - 1; i >= 0; i--)
                m_List[i].OnUpdate();
        }

        /// <inheritdoc/>
        public IEventDispatcher Create()
        {
            EventDispatcher evtSys = new EventDispatcher();
            m_List.Add(evtSys);
            return evtSys;
        }

        /// <inheritdoc/>
        public void Remove(IEventDispatcher evtSys)
        {
            m_List.Remove((EventDispatcher)evtSys);
        }
    }
}
