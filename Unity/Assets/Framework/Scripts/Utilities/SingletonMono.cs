using Mono.CSharp;
using UnityEngine;

namespace UnityXFrame.Core
{
    public class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
    {
        /// <summary>
        /// 单例实例
        /// </summary>
        public static T Inst { get; private set; }

        public SingletonMono()
        {
            Inst = (T)this;
        }
    }
}
