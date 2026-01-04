using System;
using UselessFrame.UIElements;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace UnityXFrameLib.UIElements
{
    /// <summary>
    /// 附带UI打开关闭效果的组辅助器
    /// </summary>
    public class UIGroupHelperInEffect : UIGroupHelperBase
    {
        public enum MatchUIMode
        {
            Include,
            Exclude,
        }

        private MatchUIMode m_MatchUIMode;
        private HashSet<Type> m_MathTypes;
        private List<IUIGroupHelperEffect> m_OpenEffect;
        private List<IUIGroupHelperEffect> m_CloseEffect;

        public UIGroupHelperInEffect()
        {
            m_MathTypes = new HashSet<Type>();
            m_OpenEffect = new List<IUIGroupHelperEffect>(2) { null };
            m_CloseEffect = new List<IUIGroupHelperEffect>(2) { null };
        }

        protected override void OnInit()
        {
            base.OnInit();
            m_MatchUIMode = MatchUIMode.Exclude;
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            foreach (IUIGroupHelperEffect effect in m_OpenEffect)
                effect.OnUpdate();
            foreach (IUIGroupHelperEffect effect in m_CloseEffect)
                effect.OnUpdate();
        }

        public void SetMatchMode(MatchUIMode mode)
        {
            m_MatchUIMode = mode;
        }

        public void AddMatchType(Type type)
        {
            m_MathTypes.Add(type);
        }

        protected override bool MatchType(Type type)
        {
            switch (m_MatchUIMode)
            {
                case MatchUIMode.Include: return m_MathTypes.Contains(type);
                case MatchUIMode.Exclude: return !m_MathTypes.Contains(type);
                default: return false;
            }
        }

        public void SetEffect(IUIGroupHelperEffect open, IUIGroupHelperEffect close)
        {
            m_OpenEffect[0] = open;
            m_CloseEffect[0] = close;
        }

        public void AddAlternateEffect(IUIGroupHelperEffect open, IUIGroupHelperEffect close)
        {
            m_OpenEffect.Add(open);
            m_CloseEffect.Add(close);
        }

        protected async UniTask DoOpen(IUI ui, CancellationToken token)
        {
            foreach (IUIGroupHelperEffect effect in m_OpenEffect)
            {
                if (effect == null)
                    continue;
                bool match = await effect.Do(ui, token);
                if (match)
                    return;
            }
        }

        protected async UniTask DoClose(IUI ui, CancellationToken token)
        {
            foreach (IUIGroupHelperEffect effect in m_CloseEffect)
            {
                if (effect == null)
                    continue;

                bool match = await effect.Do(ui, token);
                if (match)
                    return;
            }
        }

        protected void KillOpen(IUI ui)
        {
            foreach (IUIGroupHelperEffect effect in m_OpenEffect)
            {
                if (effect == null)
                    continue;
                if (effect.Kill(ui))
                    break;
            }
        }

        protected void KillClose(IUI ui)
        {
            foreach (IUIGroupHelperEffect effect in m_CloseEffect)
            {
                if (effect == null)
                    continue;
                if (effect.Kill(ui))
                    break;
            }
        }
    }
}
