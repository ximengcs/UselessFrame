
using IdGen;
using System;
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

        public static bool IsCoreComponent(this Type type)
        {
            return X.Type.GetAttribute(type, typeof(CoreComponentAttribute)) != null;
        }

        public static bool IsCore(this EntityComponent component)
        {
            Type type = component.GetType();
            return IsCoreComponent(type);
        }
    }
}
