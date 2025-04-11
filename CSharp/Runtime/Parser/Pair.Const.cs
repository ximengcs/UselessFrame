using System;

namespace XFrame.Core
{
    public partial struct Pair<K, V>
    {
        private static Type s_GenericType;

        static Pair()
        {
            s_GenericType = typeof(Pair<,>);
        }
    }
}
