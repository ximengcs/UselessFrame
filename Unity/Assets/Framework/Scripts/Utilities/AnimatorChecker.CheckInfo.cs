using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace UnityXFrameLib.Animations
{
    public partial class AnimatorChecker
    {
        public interface IHandle
        {
            UniTask FinishTask { get; }
        }

        private class CheckInfo : IHandle
        {
            public string Name;
            public int Layer;
            public bool IsReady;
            public Animator Anim;

            private UniTaskCompletionSource _finishTaskSource;

            public UniTask FinishTask
            {
                get
                {
                    if (_finishTaskSource == null)
                        _finishTaskSource = new UniTaskCompletionSource();
                    return _finishTaskSource.Task;
                }
            }

            public CheckInfo(Animator anim, string name, int layer)
            {
                Anim = anim;
                Name = name;
                Layer = layer;
                IsReady = false;
            }

            public void SetFinish()
            {
                if (_finishTaskSource != null)
                    _finishTaskSource.TrySetResult();
            }
        }
    }
}
