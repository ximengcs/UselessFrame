
using Google.Protobuf;
using MemoryPack;

namespace UselessFrame.NewRuntime.Entities
{
    public static class EntityMessageExtensions
    {
        public static IMessage ToCreateMessage(this Entity entity)
        {
            IMessage msg = new CreateEntityMessage()
            {
                SceneId = entity.Scene?.Id ?? 0,
                ParnetId = entity.Parent?.Id ?? 0,
                EntityId = entity.Id,
                EntityType = entity.GetType().FullName
            };
            return msg;
        }

        public static IMessage ToDestroyMessage(this Entity entity)
        {
            IMessage msg = new DestroyEntityMessage()
            {
                SceneId = entity.Scene.Id,
                EntityId = entity.Id,
            };
            return msg;
        }

        public static IMessage ToCreateMessage(this Component comp)
        {
            byte[] bytes = MemoryPackSerializer.Serialize(comp);
            CreateComponentMessage msg = new CreateComponentMessage()
            {
                SceneId = comp.Entity.Scene.Id,
                EntityId = comp.Entity.Id,
                ComponentType = comp.GetType().FullName,
                ComponentData = ByteString.CopyFrom(bytes)
            };
            return msg;
        }

        public static IMessage ToUpdateMessage(this Component comp)
        {
            byte[] bytes = MemoryPackSerializer.Serialize(comp);
            UpdateComponentMessage msg = new UpdateComponentMessage()
            {
                SceneId = comp.Entity.Scene.Id,
                EntityId = comp.Entity.Id,
                ComponentType = comp.GetType().FullName,
                ComponentData = ByteString.CopyFrom(bytes)
            };
            return msg;
        }

        public static IMessage ToDestroyMessage(this Component comp)
        {
            DestroyComponentMessage msg = new DestroyComponentMessage()
            {
                SceneId = comp.Entity.Scene.Id,
                EntityId = comp.Entity.Id,
                ComponentType = comp.GetType().FullName
            };
            return msg;
        }
    }
}
