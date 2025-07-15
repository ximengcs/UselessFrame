
using Google.Protobuf;
using MemoryPack;

namespace UselessFrame.NewRuntime.Entities
{
    public static class EntityMessageExtensions
    {
        public static IMessage CreateEntity(Entity entity)
        {
            IMessage msg = new CreateEntityMessage()
            {
                SceneId = entity.Scene.Id,
                ParnetId = entity.Parent.Id,
                EntityId = entity.Id,
                EntityType = entity.GetType().FullName
            };
            return msg;
        }

        public static IMessage DestroyEntity(Entity entity)
        {
            IMessage msg = new DestroyEntityMessage()
            {
                SceneId = entity.Scene.Id,
                EntityId = entity.Id,
            };
            return msg;
        }

        public static IMessage CreateComponent(Component comp)
        {
            byte[] bytes = MemoryPackSerializer.Serialize(comp);
            CreateComponentMessage msg = new CreateComponentMessage()
            {
                SceneId = comp.Entity.Scene.Id,
                EntityId = comp.Entity.Id,
                Component = ByteString.CopyFrom(bytes)
            };
            return msg;
        }

        public static IMessage DestroyComponent(Component comp)
        {
            DestroyComponentMessage msg = new DestroyComponentMessage()
            {
                SceneId = comp.Entity.Scene.Id,
                EntityId = comp.Entity.Id,
                Component = comp.GetType().FullName
            };
            return msg;
        }
    }
}
