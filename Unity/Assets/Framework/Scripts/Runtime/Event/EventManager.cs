using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace UselessFrame.NewRuntime.Events
{
    /// <inheritdoc/>
    public class EventManager : IManagerInitializer, IEventManager, IManagerUpdater
    {
        private List<EventDispatcher> m_List;

        public async UniTask Initialize(XSetting setting)
        {
            m_List = new List<EventDispatcher>();
        }

        /// <inheritdoc/>
        public void Update(float escapeTime)
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
