
using Unity.Mathematics;
using UselessFrame.NewRuntime.ECS;

namespace TestGame
{
    internal class TransformAwakeSystem : IAwakeSystem<TransformComponent>
    {
        public void OnAwake(TransformComponent comp)
        {
            comp.Position = comp.GetRandom().NextFloat3(new float3(-10, -10, 0), new float3(10, 10, 0));
            comp.Update();
        }
    }
}
