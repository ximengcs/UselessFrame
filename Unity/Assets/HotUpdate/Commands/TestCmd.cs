
using QFSW.QC;
using System;
using UnityEngine;
using UselessFrame.NewRuntime;

namespace Game.Commands
{
    public class TestCmd
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            QuantumRegistry.RegisterObject(new TestCmd());
        }

        [Command("t1", MonoTargetType.Registry)]
        [CommandDescription("print test logs with debug")]
        public void PrintDebug()
        {
            foreach (Type type in X.Type.Types)
            {
                Debug.Log(type.FullName);
            }
        }

        [Command("t2", MonoTargetType.Registry)]
        [CommandDescription("print test logs with warning")]
        public void PrintWarn()
        {
            Debug.LogWarning("test");
        }

        [Command("t3", MonoTargetType.Registry)]
        [CommandDescription("print test logs with error")]
        public void PrintError()
        {
            Debug.LogError("test");
        }
    }
}
