using UnityEngine;
using UselessFrame.NewRuntime;

namespace UselessFrame.UIElements
{
    public interface IUI : IContainer<IUI>
    {
        IUIHandle Handle { get; }

        RectTransform RootRect { get; }

        RectTransform MainRect { get; }

        int Layer { get; set; }

        IUIHandle Open();

        IUIHandle Close(bool destroy = true);

        IUIGroup Group { get; }
    }
}
