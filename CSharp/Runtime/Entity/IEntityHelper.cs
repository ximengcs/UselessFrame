﻿
namespace UselessFrame.NewRuntime.ECS
{
    public interface IEntityHelper
    {
        void Bind(World world);

        void OnCreateEntity(Entity entity);

        void OnDestroyEntity(Entity entity);

        void OnCreateComponent(EntityComponent component);

        void OnUpdateComponent(EntityComponent component);

        void OnDestroyComponent(EntityComponent component);
    }
}
