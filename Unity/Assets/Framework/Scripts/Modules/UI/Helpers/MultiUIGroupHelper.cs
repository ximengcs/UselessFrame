using System;
using System.Collections.Generic;
using System.Threading;
using UselessFrame.UIElements;

namespace UnityXFrameLib.UIElements
{
    /// <summary>
    /// 多UI可打开组辅助器
    /// </summary>
    public class MultiUIGroupHelper : UIGroupHelperInEffect
    {
        private HashSet<int> m_Opening;
        private HashSet<int> m_Closing;

        protected override void OnInit()
        {
            base.OnInit();
            m_Opening = new HashSet<int>();
            m_Closing = new HashSet<int>();
        }

        protected override async void OnUIOpen(IUI ui, Action openCompleteCbk, CancellationToken cancellation)
        {
            int key = ui.GetHashCode();
            if (m_Closing.Contains(key))
            {
                KillClose(ui);
                m_Closing.Remove(key);
            }

            ui.Layer = ui.Group.Count + 1;
            m_Opening.Add(key);
            await DoOpen(ui, cancellation);
            if (cancellation.IsCancellationRequested)
                return;

            if (m_Opening.Contains(key))
                openCompleteCbk();
        }

        protected override async void OnUIClose(IUI ui, Action openCompleteCbk, CancellationToken cancellation)
        {
            int key = ui.GetHashCode();
            if (m_Opening.Contains(key))
            {
                KillOpen(ui);
                m_Opening.Remove(key);
            }

            m_Closing.Add(key);
            await DoClose(ui, cancellation);
            if (cancellation.IsCancellationRequested)
                return;

            if (m_Closing.Contains(key))
            {
                m_Closing.Remove(key);
                openCompleteCbk();
            }
        }
    }
}
