
using UnityEngine;
using UselessFrame.NewRuntime.ECS;

namespace TestGame
{
    internal class PlayerEntitySystem : IAwakeSystem<PlayerEntityView>, IDestroySystem<PlayerEntityView>
    {
        public void OnAwake(PlayerEntityView comp)
        {
            Entity parent = comp.Entity.Parent;
            if (parent != null)
            {
                EntityView parentView = parent.GetComponent<EntityView>();
                comp.GameObject.transform.SetParent(parentView.GameObject.transform);
            }
        }

        public void OnDestroy(PlayerEntityView comp)
        {
            GameObject.Destroy(comp.GameObject);
        }
    }
}
