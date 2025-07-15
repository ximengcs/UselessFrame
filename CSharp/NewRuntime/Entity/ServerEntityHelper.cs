
using System;
using System.Collections.Generic;
using UselessFrame.Net;
using UselessFrame.NewRuntime.Scenes;
using UselessFrame.NewRuntime.Worlds;

namespace UselessFrame.NewRuntime.Entities
{
    public class ServerEntityHelper : IEntityHelper
    {
        private IServer _server;
        private World _world;

        internal ServerEntityHelper()
        {
            _server = IServer.Create(8888, X.MainFiber);
            _server.NewConnectionEvent += NewConnectionHandler;
        }

        public void Bind(World world)
        {
            _world = world;
        }

        private void NewConnectionHandler(IConnection connection)
        {
            Console.WriteLine("NewConnectionHandler");
            connection.State.Subscribe(ConnectionStateHandler);
        }

        private void ConnectionStateHandler(IConnection connection, ConnectionState state)
        {
            if (state == ConnectionState.Run)
            {
                Console.WriteLine("NewConnectionHandler Run");
                connection.Send(EntityMessageExtensions.CreateEntity(_world));
                foreach (Scene scene in _world.Scenes)
                {
                    connection.Send(EntityMessageExtensions.CreateEntity(scene));
                    foreach (Entity entity in scene.Entities)
                    {
                        connection.Send(EntityMessageExtensions.CreateEntity(entity));
                        foreach (Component component in entity.Components)
                        {
                            connection.Send(EntityMessageExtensions.CreateComponent(component));
                        }
                    }
                }
            }
        }

        public void Start()
        {
            _server.Start();
        }

        public void OnCreateEntity(Entity entity)
        {
            _server.Broadcast(EntityMessageExtensions.CreateEntity(entity));
        }

        public void OnDestroyEntity(Entity entity)
        {
            _server.Broadcast(EntityMessageExtensions.DestroyEntity(entity));
        }

        public void OnCreateComponent(Component component)
        {
            _server.Broadcast(EntityMessageExtensions.CreateComponent(component));
        }

        public void OnDestroyComponent(Component component)
        {
            _server.Broadcast(EntityMessageExtensions.DestroyComponent(component));
        }
    }
}
