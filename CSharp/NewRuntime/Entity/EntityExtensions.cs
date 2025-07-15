
using IdGen;
using UselessFrame.NewRuntime.Worlds;

namespace UselessFrame.NewRuntime.Entities
{
    public static class EntityExtensions
    {
        public const int INVALID_ID = 0;

        public static IdGenerator IdGen(this Entity entity)
        {
            if (entity.Scene != null)
            {
                if (entity.Scene.World != null)
                {
                    return entity.Scene.World.IdGen;
                }
            }

            World world = entity as World;
            if (world != null)
                return world.IdGen;
            return null;
        }
    }
}
