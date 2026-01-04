
using UnityEngine;
using UselessFrame.NewRuntime.ECS;

namespace TestGame
{
    internal class EntityViewSystem : IAwakeSystem<EntityView>, IDestroySystem<EntityView>
    {
        public void OnAwake(EntityView comp)
        {
            Entity parent = comp.Entity.Parent;
            if (parent != null)
            {
                EntityView parentView = parent.GetComponent<EntityView>();
                comp.GameObject.transform.SetParent(parentView.GameObject.transform);
            }
        }

        public void OnDestroy(EntityView comp)
        {
            GameObject.Destroy(comp.GameObject);
        }
    }
}
