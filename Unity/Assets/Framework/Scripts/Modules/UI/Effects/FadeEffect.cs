using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UselessFrame.UIElements;

namespace UnityXFrameLib.UIElements
{
    /// <summary>
    /// 渐隐渐显效果
    /// </summary>
    public class FadeEffect : IUIGroupHelperEffect
    {
        private float m_Dur;
        private float m_Start;
        private float m_Target;
        private Dictionary<int, Tween> m_Anims;

        public FadeEffect(float target, float duration = 0.2f)
        {
            m_Start = 0;
            m_Target = target;
            m_Dur = duration;
            m_Anims = new Dictionary<int, Tween>();
        }

        public FadeEffect(float start, float target, float duration = 0.2f)
        {
            m_Start = start;
            m_Target = target;
            m_Dur = duration;
            m_Anims = new Dictionary<int, Tween>();
        }

        void IUIGroupHelperEffect.OnUpdate()
        {

        }

        public async UniTask<bool> Do(IUI ui, CancellationToken token)
        {
            int key = ui.GetHashCode();
            CanvasGroup canvasGroup = InnerEnsureCanvasGroup(ui);
            Tween tween = canvasGroup.DOAlpha(m_Target, m_Dur);
            m_Anims.Add(key, tween);
            AutoResetUniTaskCompletionSource taskSource = AutoResetUniTaskCompletionSource.Create();
            tween.OnComplete(() => taskSource.TrySetResult());
            m_Anims.Remove(key);
            return true;
        }

        public bool Kill(IUI ui)
        {
            int key = ui.GetHashCode();
            if (m_Anims.TryGetValue(key, out Tween tween))
            {
                tween.Kill();
                m_Anims.Remove(key);
            }
            return true;
        }

        private CanvasGroup InnerEnsureCanvasGroup(IUI ui)
        {
            CanvasGroup canvasGroup = ui.MainRect.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = ui.MainRect.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = m_Start;
            return canvasGroup;
        }
    }
}
