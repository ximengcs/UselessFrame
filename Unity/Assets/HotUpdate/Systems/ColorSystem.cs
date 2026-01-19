
using Unity.Mathematics;
using UnityEngine;
using UselessFrame.NewRuntime.ECS;
using UselessFrameCommon.Entities;

namespace TestGame
{
    internal class ColorSystem : IAwakeSystem<ColorView>, IDestroySystem<ColorView>
    {
        public void OnAwake(ColorView comp)
        {
            ColorComponent colorComp = comp.GetComponent<ColorComponent>();
            int4 c = colorComp.Color;
            comp.Render = comp.GetComponent<EntityView>().GameObject.AddComponent<SpriteRenderer>();
            comp.Render.sprite = Resources.Load<Sprite>("square");
            comp.Render.color = new Color(c.x / 255f, c.y / 255f, c.z / 255f, c.w / 255f);
        }

        public void OnDestroy(ColorView comp)
        {
            Debug.Log($"ColorViewSystem OnDestroy");
            GameObject.Destroy(comp.Render);
            comp.Render = null;
        }
    }
}
