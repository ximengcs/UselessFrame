
using QFSW.QC;
using UnityEngine;

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
            Debug.Log("test");
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
