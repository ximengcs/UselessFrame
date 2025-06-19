
namespace UselessFrame.NewRuntime.Fiber
{
    public partial class Fiber
    {
        internal struct LoopItemInfo
        {
            public IFiberLoopItem Item;
            public bool ShouldRemove;

            public LoopItemInfo(IFiberLoopItem item)
            {
                Item = item;
                ShouldRemove = false;
            }
        }
    }
}
