
using Google.Protobuf;
using MemoryPack;
using System;

namespace UselessFrame.NewRuntime.ECS
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
                SceneId = entity.Scene?.Id ?? 0,
                EntityId = entity.Id,
            };
            return msg;
        }

        public static IMessage ToCreateMessage(this EntityComponent comp)
        {
            byte[] bytes = MemoryPackSerializer.Serialize(comp.GetType(), comp);
            CreateComponentMessage msg = new CreateComponentMessage()
            {
                SceneId = comp.Entity.Scene?.Id ?? 0,
                EntityId = comp.Entity.Id,
                ComponentType = comp.GetType().FullName,
                ComponentData = ByteString.CopyFrom(bytes)
            };
            return msg;
        }

        public static IMessage ToUpdateMessage(this EntityComponent comp)
        {
            byte[] bytes = MemoryPackSerializer.Serialize(comp.GetType(), comp);
            UpdateComponentMessage msg = new UpdateComponentMessage()
            {
                SceneId = comp.Entity.Scene?.Id ?? 0,
                EntityId = comp.Entity.Id,
                ComponentType = comp.GetType().FullName,
                ComponentData = ByteString.CopyFrom(bytes)
            };
            return msg;
        }

        public static IMessage ToDestroyMessage(this EntityComponent comp)
        {
            DestroyComponentMessage msg = new DestroyComponentMessage()
            {
                SceneId = comp.Entity.Scene?.Id ?? 0,
                EntityId = comp.Entity.Id,
                ComponentType = comp.GetType().FullName
            };
            return msg;
        }
    }
}
