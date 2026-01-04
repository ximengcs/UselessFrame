using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UselessFrame.NewRuntime;
using UselessFrame.UIElements;

namespace UnityXFrameLib.UIElements
{
    /// <summary>
    /// 移动效果
    /// </summary>
    public class MoveEffect : IUIGroupHelperEffect
    {
        public enum Direct
        {
            FromTop,
            FromBottom,
            FromLeft,
            FromRight,
            Rand
        }

        private float m_Dur;
        private bool m_IsOpen;
        private Direct m_Direct;
        private bool m_CompleteReset;
        private Dictionary<int, Tween> m_Anims;

        public MoveEffect(Direct direct, bool open, bool completeReset, float duration = 0.2f)
        {
            m_IsOpen = open;
            m_Direct = direct;
            m_Dur = duration;
            m_CompleteReset = completeReset;
            m_Anims = new Dictionary<int, Tween>();
        }

        void IUIGroupHelperEffect.OnUpdate()
        {

        }

        public async UniTask<bool> Do(IUI ui, CancellationToken token)
        {
            int key = ui.GetHashCode();
            Vector2 start;
            Vector2 end;
            Direct direct = m_Direct;
            if (m_Direct == Direct.Rand)
                direct = X.Random.NextEnum(Direct.Rand);
            switch (direct)
            {
                case Direct.FromLeft:
                    start = new Vector2(-ui.MainRect.sizeDelta.x, 0);
                    end = Vector2.zero;
                    break;

                case Direct.FromRight:
                    start = new Vector2(ui.MainRect.sizeDelta.x, 0);
                    end = Vector2.zero;
                    break;

                case Direct.FromTop:
                    start = new Vector2(0, ui.MainRect.sizeDelta.y);
                    end = Vector2.zero;
                    break;

                case Direct.FromBottom:
                    start = new Vector2(0, -ui.MainRect.sizeDelta.y);
                    end = Vector2.zero;
                    break;

                default:
                    start = Vector2.zero;
                    end = Vector2.zero;
                    break;
            }

            if (!m_IsOpen)
            {
                Vector2 tmp = start;
                start = end;
                end = tmp;
            }

            ui.MainRect.anchoredPosition = start;

            Tween tween = ui.MainRect.DOAnchoredPos(end, m_Dur);
            m_Anims.Add(key, tween);
            AutoResetUniTaskCompletionSource taskSource = AutoResetUniTaskCompletionSource.Create();
            tween.OnComplete(() => taskSource.TrySetResult());
            await taskSource.Task;
            m_Anims.Remove(key);
            if (m_CompleteReset)
            {
                tween.OnKill(() =>
                {
                    ui.MainRect.anchoredPosition = start;
                });
            }
            return !token.IsCancellationRequested;
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
    }
}
