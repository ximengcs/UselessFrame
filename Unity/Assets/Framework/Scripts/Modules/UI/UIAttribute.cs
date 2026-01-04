
using System;

namespace UselessFrame.UIElements
{
    public class UIAttribute : Attribute
    {
        public int ResSource { get; }

        public UIAttribute(int resSource)
        {
            ResSource = resSource;
        }
    }
}
