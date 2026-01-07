
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UselessFrame.Runtime.Types;

namespace UnityXFrame.Core.Diagnotics
{
    internal class TypeFilter : ITypeFilter
    {
        private string[] _validList = new string[]
        {
            "Framework",
            "UselessFrame",
            "Assembly-CSharp",
            "Common",
        };

        public bool CheckAssembly(string assemblyName)
        {
            return _validList.Contains(assemblyName);
        }

        public bool CheckType(Type type)
        {
            return true;
        }
    }
}
