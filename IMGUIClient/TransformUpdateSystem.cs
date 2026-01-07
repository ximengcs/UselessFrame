
using UselessFrame.NewRuntime.ECS;

namespace TestGame
{
    internal class TransformUpdateSystem : IUpdateSystem<TransformComponent>
    {
        public void OnUpdate(TransformComponent oldComp, TransformComponent newComp)
        {
            Console.WriteLine($"TransformUpdateSystem OnUpdate {newComp.Position}");
        }
    }
}
