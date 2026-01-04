using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UselessFrame.UIElements;
using UnityXFrame.Core.UIElements;

namespace UnityXFrameLib.UIElements
{
    public static class UIModuleExt
    {
        public static void AddButtonListener(this IUI ui, string buttonName, UnityAction callback)
        {
            ui.GetCom<UIComFinder>().GetUI<Button>(buttonName).onClick.AddListener(callback);
        }

        public static void RemoveButtonListener(this IUI ui, string buttonName, UnityAction callback)
        {
            ui.GetCom<UIComFinder>().GetUI<Button>(buttonName).onClick.RemoveListener(callback);
        }

        public static void RemoveButtonListeners(this IUI ui, string buttonName)
        {
            ui.GetCom<UIComFinder>().GetUI<Button>(buttonName).onClick.RemoveAllListeners();
        }

        public static Tween DOAnchoredPos(this RectTransform tf, Vector2 target, float duration)
        {
            return DOTween.To(() => tf.anchoredPosition, (pos) => tf.anchoredPosition = pos, target, duration);
        }

        public static Tween DOAlpha(this CanvasGroup group, float target, float duration)
        {
            return DOTween.To(() => group.alpha, (pos) => group.alpha = pos, target, duration);
        }
    }
}
