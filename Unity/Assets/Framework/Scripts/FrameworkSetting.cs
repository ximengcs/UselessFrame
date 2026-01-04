
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UselessFrameUnity
{
    public class FrameworkSetting : ScriptableObject
    {
        public List<DebugColor> LogMark;
    }

    [Serializable]
    public class DebugColor
    {
        public string Key;
        public Color Color;
        public bool Value;
    }
}
