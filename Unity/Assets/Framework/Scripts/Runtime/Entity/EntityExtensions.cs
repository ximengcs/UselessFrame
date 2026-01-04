
using IdGen;
using System;
using UselessFrame.NewRuntime.Randoms;

namespace UselessFrame.NewRuntime.ECS
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

        public static void Update(this EntityComponent component)
        {
            component.Entity.UpdateComponent(component);
        }

        public static T GetComponent<T>(this EntityComponent component) where T : EntityComponent
        {
            return component.Entity.GetComponent<T>();
        }

        public static IRandom GetRandom(this EntityComponent component)
        {
            return component.Entity.Scene.World.Random;
        }
    }
}
