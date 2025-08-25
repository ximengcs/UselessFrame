using System;
using MemoryPack;
using Google.Protobuf;
using UselessFrame.Net;
using System.Collections.Generic;

namespace UselessFrame.NewRuntime.ECS
{
    internal class ClientEntityHelper : IEntityHelper, ICombineEntityHelper
    {
        private IConnection _connection;
        private World _world;
        private Dictionary<Type, Action<IMessage>> _handles;
        private IEntityHelper _helper;

        public World World => _world;

        public INetNode NetNode => _connection;

        public ClientEntityHelper(IConnection connection)
        {
            _connection = connection;
        }

        public void Trigger(IMessage message)
        {
            _connection.Send(message);
        }

        public void Bind(World world)
        {
            _world = world;
            _helper?.Bind(world);
            _handles = new Dictionary<Type, Action<IMessage>>()
            {
                { typeof(CreateEntityMessage), CreateEntity },
                { typeof(DestroyEntityMessage), DestoryEntity },
                { typeof(CreateComponentMessage), CreateComponent },
                { typeof(UpdateComponentMessage), UpdateComponent },
                { typeof(DestroyComponentMessage), DestoryComponent },
            };
            _connection.ReceiveMessageEvent += TriggerMessage;
        }

        public void AddHelper(IEntityHelper helper)
        {
            _helper = helper;
            if (_world != null)
                _helper.Bind(_world);
        }

        private void TriggerMessage(MessageResult result)
        {
            if (result.Valid)
            {
                if (_handles.TryGetValue(result.MessageType, out var handle))
                {
                    handle(result.Message);
                }
                else
                {
                    _world.Event.TriggerMessage(result.Message);
                }
            }
        }

        private void DestoryEntity(IMessage message)
        {
            DestroyEntityMessage createMsg = (DestroyEntityMessage)message;
            long sceneId = createMsg.SceneId;
            if (sceneId != EntityExtensions.INVALID_ID)
            {
                Scene scene = _world.GetScene(sceneId);
                if (scene != null)
                {
                    Entity entity = scene.FindEntity(createMsg.EntityId);
                    if (entity.Parent != null)
                        entity.Parent.RemoveEntity(entity.Id);
                }
            }
        }

        private void CreateEntity(IMessage message)
        {
            CreateEntityMessage createMsg = (CreateEntityMessage)message;
            long sceneId = createMsg.SceneId;
            long parentId = createMsg.ParnetId;
            long entityId = createMsg.EntityId;
            if (sceneId != EntityExtensions.INVALID_ID)
            {
                Scene scene = _world.GetScene(sceneId);
                if (scene == null)
                    scene = _world.AddEntity<Scene>(sceneId);
                if (entityId != EntityExtensions.INVALID_ID && parentId != EntityExtensions.INVALID_ID)
                {
                    Entity parent = null;
                    if (parentId == sceneId)
                        parent = scene;
                    else
                        parent = scene.FindEntity(parentId);

                    if (parent != null)
                    {
                        string entityTypeName = createMsg.EntityType;
                        if (X.Type.TryGetType(entityTypeName, out Type type))
                        {
                            parent.AddEntity(type, entityId);
                        }
                    }
                }
            }
            else
            {
                if (parentId == EntityExtensions.INVALID_ID && entityId != EntityExtensions.INVALID_ID)
                {
                    string entityTypeName = createMsg.EntityType;
                    if (X.Type.TryGetType(entityTypeName, out Type type) && type == typeof(World))
                    {
                        _world.Id = entityId;
                        OnCreateEntity(_world);
                    }
                }
                else if (parentId != EntityExtensions.INVALID_ID && parentId == _world.Id)
                {
                    string entityTypeName = createMsg.EntityType;
                    if (X.Type.TryGetType(entityTypeName, out Type type))
                    {
                        _world.AddEntity(type, entityId);
                    }
                }
            }
        }

        private void CreateComponent(IMessage message)
        {
            CreateComponentMessage createMsg = (CreateComponentMessage)message;
            long sceneId = createMsg.SceneId;
            if (sceneId != EntityExtensions.INVALID_ID)
            {
                Scene scene = _world.GetScene(sceneId);
                if (scene != null)
                {
                    Entity entity = scene.FindEntity(createMsg.EntityId);
                    if (entity != null)
                    {
                        if (X.Type.TryGetType(createMsg.ComponentType, out Type compType))
                        {
                            EntityComponent comp = (EntityComponent)MemoryPackSerializer.Deserialize(compType, createMsg.ComponentData.Span);
                            entity.AddOrUpdateComponent(comp);
                        }
                        else
                        {
                            X.Log.Error($"add component error, component type is null {createMsg.ComponentType}");
                        }
                    }
                    else
                    {
                        X.Log.Error($"add component error, entity is null {createMsg.EntityId}");
                    }
                }
                else
                {
                    X.Log.Error($"add component error, scene is null {sceneId}");
                }
            }
        }

        private void UpdateComponent(IMessage message)
        {
            UpdateComponentMessage createMsg = (UpdateComponentMessage)message;
            long sceneId = createMsg.SceneId;
            if (sceneId != EntityExtensions.INVALID_ID)
            {
                Scene scene = _world.GetScene(sceneId);
                if (scene != null)
                {
                    Entity entity = scene.FindEntity(createMsg.EntityId);
                    if (entity != null)
                    {
                        if (X.Type.TryGetType(createMsg.ComponentType, out Type compType))
                        {
                            EntityComponent comp = (EntityComponent)MemoryPackSerializer.Deserialize(compType, createMsg.ComponentData.Span);
                            entity.UpdateComponent(comp);
                        }
                        else
                        {
                            X.Log.Error($"add component error, component type is null {createMsg.ComponentType}");
                        }
                    }
                    else
                    {
                        X.Log.Error($"add component error, entity is null {createMsg.EntityId}");
                    }
                }
                else
                {
                    X.Log.Error($"add component error, scene is null {sceneId}");
                }
            }
        }

        public void DestoryComponent(IMessage message)
        {
            DestroyComponentMessage createMsg = (DestroyComponentMessage)message;
            long sceneId = createMsg.SceneId;
            if (sceneId != EntityExtensions.INVALID_ID)
            {
                Scene scene = _world.GetScene(sceneId);
                if (scene != null)
                {
                    Entity entity = scene.FindEntity(createMsg.EntityId);
                    if (entity != null)
                    {
                        if (X.Type.TryGetType(createMsg.ComponentType, out Type compType))
                        {
                            entity.RemoveComponent(compType);
                        }
                        else
                        {
                            X.Log.Error($"add component error, component type is null {createMsg.ComponentType}");
                        }
                    }
                    else
                    {
                        X.Log.Error($"add component error, entity is null {createMsg.EntityId}");
                    }
                }
                else
                {
                    X.Log.Error($"add component error, scene is null {sceneId}");
                }
            }
        }

        public void UnBind()
        {
            _connection.ReceiveMessageEvent -= TriggerMessage;
        }

        public void OnDestroyEntity(Entity entity)
        {
            _helper?.OnDestroyEntity(entity);
        }

        public void OnCreateEntity(Entity entity)
        {
            _helper?.OnCreateEntity(entity);
        }

        public void OnCreateComponent(EntityComponent component)
        {
            _helper?.OnCreateComponent(component);
        }

        public void OnUpdateComponent(EntityComponent component)
        {
            _helper?.OnUpdateComponent(component);
        }

        public void OnDestroyComponent(EntityComponent component)
        {
            _helper?.OnDestroyComponent(component);
        }
    }
}
