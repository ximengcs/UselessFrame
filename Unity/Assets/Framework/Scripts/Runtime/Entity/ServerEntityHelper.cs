
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Data.Common;
using UselessFrame.Net;
using UselessFrame.NewRuntime.Fiber;

namespace UselessFrame.NewRuntime.ECS
{
    public class ServerEntityHelper : IEntityHelper, ICombineEntityHelper
    {
        private IServer _server;
        private World _world;
        private IEntityHelper _helper;
        private Dictionary<long, long> _playersById;

        public World World => _world;

        public INetNode NetNode => _server;

        internal ServerEntityHelper(int port, IFiber fiber)
        {
            _server = X.Net.Create(port, fiber);
            _server.NewConnectionEvent += NewConnectionHandler;
            _server.Start();
        }

        public void Trigger(IMessage message)
        {
            _server.Broadcast(message);
        }

        public void AddHelper(IEntityHelper helper)
        {
            _helper = helper;
            if (_world != null)
                _helper.Bind(_world);
        }

        public void Bind(World world)
        {
            _playersById = new Dictionary<long, long>();
            _world = world;
            _helper?.Bind(world);
        }

        private void NewConnectionHandler(IConnection connection)
        {
            if (!_playersById.ContainsKey(connection.Id))
            {
                PlayerEntity entity = _world.AddEntity<PlayerEntity>();
                IdComponent idComp = entity.GetOrAddComponent<IdComponent>();
                idComp.Id = connection.Id;
                _playersById.Add(connection.Id, entity.Id);
            }

            connection.State.Subscribe(ConnectionStateHandler);
            connection.ReceiveMessageEvent += TriggerMessage;
        }

        private void TriggerMessage(MessageResult result)
        {
            if (result.Valid)
            {
                _world.Event.TriggerMessage(result.Message);
            }
        }

        private void ConnectionStateHandler(IConnection connection, ConnectionState state)
        {
            switch (state)
            {
                case ConnectionState.Run:
                    {
                        RecursiveSyncEntity(connection, _world);
                        break;
                    }

                case ConnectionState.Dispose:
                    {
                        if (_playersById.TryGetValue(connection.Id, out long playerEntityId))
                        {
                            _world.RemoveEntity(playerEntityId);
                        }
                        break;
                    }
            }
        }

        private void RecursiveSyncEntity(IConnection connection, Entity entity)
        {
            connection.Send(entity.ToCreateMessage());

            foreach (EntityComponent component in entity.Components)
            {
                connection.Send(component.ToCreateMessage());
            }

            foreach (Entity child in entity.Entities)
            {
                RecursiveSyncEntity(connection, child);
            }
        }

        public void OnCreateEntity(Entity entity)
        {
            if (entity == _world)
                _world.InitId();

            _server.Broadcast(entity.ToCreateMessage());
            _helper?.OnCreateEntity(entity);
        }

        public void OnDestroyEntity(Entity entity)
        {
            _server.Broadcast(entity.ToDestroyMessage());
            _helper?.OnDestroyEntity(entity);
        }

        public void OnCreateComponent(EntityComponent component)
        {
            if (component.Entity.IsDisposed) return;
            _server.Broadcast(component.ToCreateMessage());
            _helper?.OnCreateComponent(component);
        }

        public void OnUpdateComponent(EntityComponent component)
        {
            if (component.Entity.IsDisposed) return;
            _server.Broadcast(component.ToUpdateMessage());
            _helper?.OnUpdateComponent(component);
        }

        public void OnDestroyComponent(EntityComponent component)
        {
            if (component.Entity.IsDisposed) return;
            _server.Broadcast(component.ToDestroyMessage());
            _helper?.OnDestroyComponent(component);
        }
    }
}
