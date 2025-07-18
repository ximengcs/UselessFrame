
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
            connection.State.Subscribe(ConnectionStateHandler);
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
            foreach (Entity child in entity.Entities)
            {
                RecursiveSyncEntity(connection, child);
                foreach (Component component in entity.Components)
                {
                    connection.Send(component.ToCreateMessage());
                }
            }
        }

        public void Start()
        {
            _server.Start();
        }

        public void OnCreateEntity(Entity entity)
        {
            _server.Broadcast(entity.ToCreateMessage());
        }

        public void OnDestroyEntity(Entity entity)
        {
            _server.Broadcast(entity.ToDestroyMessage());
        }

        public void OnCreateComponent(Component component)
        {
            _server.Broadcast(component.ToCreateMessage());
        }

        public void OnUpdateComponent(Component component)
        {
            _server.Broadcast(component.ToUpdateMessage());
        }

        public void OnDestroyComponent(Component component)
        {
            _server.Broadcast(component.ToDestroyMessage());
        }
    }
}
