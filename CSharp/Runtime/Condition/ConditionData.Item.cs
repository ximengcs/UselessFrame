
namespace XFrame.Modules.Conditions
{
    public partial struct ConditionData
    {
        public struct Item
        {
            public readonly bool Valid;
            public readonly int Type;
            public readonly Param Value;

            public Item(int type, string value)
            {
                Type = type;
                Value = new Param(value);
                Valid = true;
            }
        }
    }
}
