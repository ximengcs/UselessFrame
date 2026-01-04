
using UnityEngine;
using UselessFrame.NewRuntime.ECS;

namespace TestGame
{
    internal class SceneViewSystem : IAwakeSystem<SceneView>, IDestroySystem<SceneView>
    {
        public void OnAwake(SceneView comp)
        {
            Entity parent = comp.Entity.Parent;
            if (parent != null)
            {
                EntityView parentView = parent.GetComponent<EntityView>();
                Debug.Log($"SceneViewSystem {parentView == null} {parent.GetType().Name} {parent.GetHashCode()}");
                comp.GameObject.transform.SetParent(parentView.GameObject.transform);
            }
        }

        public void OnDestroy(SceneView comp)
        {
            GameObject.Destroy(comp.GameObject);
        }
    }
}
