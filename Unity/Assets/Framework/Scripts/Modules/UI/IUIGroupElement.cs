
namespace UselessFrame.UIElements
{
    internal interface IUIGroupElement : IUI, IUINode
    {
        new UIHandle Handle { get; }

        void OnBindHandle(long id, UIHandle handle);

        void OnInit(object userData);

        void OnOpen();

        void OnClose();

        void OnUpdate();

        void OnGroupChange();

        void OnSetGroup(IUIGroup group);

        void OnSetLayer(int layer);
    }
}
