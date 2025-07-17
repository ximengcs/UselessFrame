using Google.Protobuf;
using System;
using System.Collections.Generic;
using UselessFrame.Net;
using UselessFrame.NewRuntime.Scenes;
using UselessFrame.NewRuntime.Worlds;

namespace UselessFrame.NewRuntime.Entities
{
    internal class ClientEntityHelper : IEntityHelper, ICanNet
    {
        private IConnection _connection;
        private World _world;
        private Dictionary<Type, Action<IMessage>> _handles;

        IConnection ICanNet.Connection => _connection;

        public ClientEntityHelper(IConnection connection)
        {
            _connection = connection;
        }

        public void Bind(World world)
        {
            _world = world;
            _handles = new Dictionary<Type, Action<IMessage>>()
            {
                { typeof(CreateEntityMessage), CreateEntity }
            };
            _connection.ReceiveMessageEvent += TriggerMessage;
        }

        private void TriggerMessage(MessageResult result)
        {
            if (result.Valid)
            {
                if (_handles.TryGetValue(result.MessageType, out var handle))
                {
                    handle(result.Message);
                }
            }
        }

        private void CreateEntity(IMessage message)
        {
            CreateEntityMessage createMsg = (CreateEntityMessage)message;
            long sceneId = createMsg.SceneId;
            if (sceneId != EntityExtensions.INVALID_ID)
            {
                Scene scene = _world.GetScene(sceneId);
                if (scene == null)
                    scene = _world.AddEntity<Scene>(sceneId);
                long parentId = createMsg.ParnetId;
                long entityId = createMsg.EntityId;
                if (entityId != EntityExtensions.INVALID_ID && parentId != EntityExtensions.INVALID_ID)
                {
                    Entity parent = null;
                    if (parentId == sceneId)
                        parent = scene;
                    else
                        parent = scene.GetEntity(parentId);

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

            }
        }

        public void UnBind()
        {
            _connection.ReceiveMessageEvent -= TriggerMessage;
        }

        public void OnDestroyEntity(Entity entity)
        {

        }

        public void OnCreateEntity(Entity entity)
        {

        }

        public void OnCreateComponent(Component component)
        {

        }

        public void OnDestroyComponent(Component component)
        {

        }
    }
}
