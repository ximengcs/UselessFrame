﻿
using System;
using UselessFrame.Net;
using UselessFrame.NewRuntime.Fiber;

namespace UselessFrame.NewRuntime.ECS
{
    public class ServerEntityHelper : IEntityHelper, ICombineEntityHelper
    {
        private IServer _server;
        private World _world;
        private IEntityHelper _helper;

        internal ServerEntityHelper(int port, IFiber fiber)
        {
            _server = X.Net.Create(port, fiber);
            _server.NewConnectionEvent += NewConnectionHandler;
            _server.Start();
        }

        public void AddHelper(IEntityHelper helper)
        {
            _helper = helper;
            if (_world != null)
                _helper.Bind(_world);
        }

        public void Bind(World world)
        {
            _world = world;
            _helper?.Bind(world);
        }

        private void NewConnectionHandler(IConnection connection)
        {
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
            if (state == ConnectionState.Run)
            {
                connection.Send(_world.ToCreateMessage());
                foreach (Scene scene in _world.Scenes)
                {
                    RecursiveSyncEntity(connection, scene);
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
            Console.WriteLine($"OnCreateComponent {component.Entity.Id} {component.GetType().Name}");
            _server.Broadcast(component.ToCreateMessage());
            _helper?.OnCreateComponent(component);
        }

        public void OnUpdateComponent(EntityComponent component)
        {
            if (component.Entity.IsDisposed) return;
            Console.WriteLine($"OnUpdateComponent {component.Entity.Id} {component.GetType().Name}");
            _server.Broadcast(component.ToUpdateMessage());
            _helper?.OnUpdateComponent(component);
        }

        public void OnDestroyComponent(EntityComponent component)
        {
            if (component.Entity.IsDisposed) return;
            Console.WriteLine($"OnDestroyComponent {component.Entity.Id} {component.GetType().Name}");
            _server.Broadcast(component.ToDestroyMessage());
            _helper?.OnDestroyComponent(component);
        }
    }
}
