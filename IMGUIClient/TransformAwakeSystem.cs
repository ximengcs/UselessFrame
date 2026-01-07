
using UselessFrame.NewRuntime.ECS;

namespace TestGame
{
    internal class TransformAwakeSystem : IAwakeSystem<TransformComponent>
    {
        public void OnAwake(TransformComponent comp)
        {
            Console.WriteLine($"TransformAwakeSystem OnAwake {comp.Position}");
        }
    }
}
