using Core.Application;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using ImGuiNET;
using System.Numerics;
using UselessFrame.Net;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.ECS;
using UselessFrame.NewRuntime.Fiber;
using UselessFrame.Runtime;
using UselessFrameCommon.Entities;

namespace TestIMGUI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Perch.Do();
            X.Initialize(InitFrameSetting()).GetAwaiter().GetResult();

            _app = IApp.Create("Server", 600, 400);
            _app.OnUpdate(X.Update);
            _app.OnGUI(OnGUI);
            FiberUtility.RunLoopSleep1(_app.Update, _app.DisposeToken, out _);
        }

        private static XSetting InitFrameSetting()
        {
            XSetting setting = new XSetting();
            setting.Loggers = new[] { new ConsoleLogger() };
            return setting;
        }

        private static IApp _app;
        private static IServer _server;
        private static IConnection _selectClient;
        private static string _msg = string.Empty;
        private static int _port = 9999;
        private static List<IMessage> listCache = new List<IMessage>();
        private static Queue<IMessage> _msgList = new Queue<IMessage>(20);
        private static World _world;
        private static int _compRef = 0;
        private static int _entityRef = 0;

        private static void OnReceiveMsg(MessageResult msg)
        {
            if (_msgList.Count >= 20)
                _msgList.Dequeue();
            _msgList.Enqueue(msg.Message);
        }

        private static void OnNewConnection(IConnection connection)
        {
            connection.ReceiveMessageEvent += OnReceiveMsg;
        }

        private static void OnGUI()
        {
            ImGui.SetNextWindowPos(new Vector2(0, 0));
            ImGui.SetNextWindowSize(new Vector2(300, 400));
            ImGui.Begin("Message", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.HorizontalScrollbar);
            lock (listCache)
            {
                listCache.Clear();
                lock (_msgList)
                    listCache.AddRange(_msgList);

                foreach (IMessage msg in listCache)
                    ImGui.Text(msg.ToString());
            }
            ImGui.End();

            ImGui.SetNextWindowPos(new Vector2(300, 0));
            ImGui.SetNextWindowSize(new Vector2(300, 400));
            ImGui.Begin("Server", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.HorizontalScrollbar);
            if (_server == null)
            {
                ImGui.InputInt("Port", ref _port);
                if (ImGui.Button("Start Server"))
                {
                    _server = X.Net.Create(_port, X.Fiber.MainFiber);
                    _server.NewConnectionEvent += OnNewConnection;
                    _server.Start();
                }
            }
            else
            {
                switch (_server.State.Value)
                {
                    case ServerState.Listen:
                        {
                            ImGui.TextColored(AppUtility.Cyan, $"{_server.Host.Address}:{_server.Host.Port} [{_server.State.Value}]");

                            if (ImGui.Button("create world"))
                            {
                                _world = X.World.Create(WorldSetting.Server(8888));
                                Scene scene = _world.AddEntity<Scene>();
                                for (int i = 0; i < 1; i++)
                                {
                                    Entity entity = scene.AddEntity<Entity>();
                                    entity.AddEntity<Entity>();
                                    entity.GetOrAddComponent<ColorComponent>();
                                }
                            }
                            if (ImGui.Button("create entity"))
                            {
                                TestCreateEntity().Forget();
                            }
                            if (ImGui.Button("print entity tree"))
                            {
                                Console.WriteLine(_world);
                            }

                            RenderWorld();

                            foreach (IConnection connection in _server.Connections)
                            {
                                switch (connection.State.Value)
                                {
                                    case ConnectionState.Run:
                                        ImGui.PushStyleColor(ImGuiCol.Text, AppUtility.Green);
                                        break;

                                    default:
                                        ImGui.PushStyleColor(ImGuiCol.Text, AppUtility.Red);
                                        break;
                                }

                                if (ImGui.Button(connection.Id.ToString()))
                                {
                                    _selectClient = connection;
                                    _app.Resize(900, 400);
                                }
                                ImGui.PopStyleColor();
                            }

                            if (ImGui.Button("Close Server"))
                            {
                                _server.Close();
                                _selectClient = null;
                            }
                        }

                        break;

                    case ServerState.Close:
                        ImGui.TextColored(AppUtility.LightBlue, $"{_server.Host.Address}:{_server.Host.Port} [{_server.State.Value}]");

                        if (ImGui.Button("Remove Server"))
                        {
                            _server = null;
                        }
                        break;

                    default:
                        ImGui.TextColored(AppUtility.Red, $"{_server.Host} [{_server.State.Value}]");

                        if (ImGui.Button("Remove Server"))
                        {
                            _server = null;
                        }
                        break;
                }
            }
            ImGui.End();

            if (_selectClient != null)
            {
                ImGui.SetNextWindowPos(new Vector2(600, 0));
                ImGui.SetNextWindowSize(new Vector2(300, 400));
                ImGui.Begin("Client", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.HorizontalScrollbar);
                switch (_selectClient.State.Value)
                {
                    case ConnectionState.Run:
                        ImGui.TextColored(AppUtility.Green, $"{_selectClient.State.Value}");
                        ImGui.TextColored(AppUtility.LightGreen, $"{_selectClient.Id}");
                        ImGui.BeginGroup();
                        ImGui.InputText("##Message", ref _msg, 20);
                        ImGui.SameLine();
                        if (ImGui.Button("Send"))
                            _selectClient.Send(new StringMessage() { Content = _msg }).Forget();
                        if (ImGui.Button("LoopSendTo"))
                            LoopSendTo(_selectClient).Forget();
                        ImGui.EndGroup();
                        break;

                    case ConnectionState.Connect:
                    case ConnectionState.CheckConnect:
                    case ConnectionState.TokenVerify:
                    case ConnectionState.TokenResponse:
                        ImGui.TextColored(AppUtility.LightBlue, $"[{_selectClient.State.Value}]");
                        break;

                    case ConnectionState.CloseRequest:
                    case ConnectionState.CloseResponse:
                    case ConnectionState.Dispose:
                        ImGui.TextColored(AppUtility.Red, $"[{_selectClient.State.Value}]");
                        ImGui.TextColored(AppUtility.Yellow, $"server may not start.");
                        //if (ImGui.Button("Reconnect"))
                        //    _selectClient.Reconnect();
                        break;

                    default: ImGui.TextColored(AppUtility.Red, $"[{_selectClient.State.Value}]"); break;
                }

                switch (_selectClient.State.Value)
                {
                    case ConnectionState.CloseRequest:
                    case ConnectionState.CloseResponse:
                    case ConnectionState.Dispose:
                        ImGui.PushStyleColor(ImGuiCol.Text, AppUtility.LightRed);
                        if (ImGui.Button("Remove Client"))
                        {
                            _selectClient = null;
                            _app.Resize(600, 400);
                        }
                        ImGui.PopStyleColor();
                        break;

                    default:
                        ImGui.PushStyleColor(ImGuiCol.Text, AppUtility.Red);
                        if (ImGui.Button("Close Connect"))
                            _selectClient.Close();
                        ImGui.PopStyleColor();
                        break;
                }
            }
        }

        private static void RenderWorld()
        {
            if (_world != null)
            {
                if (ImGui.TreeNode($"[{_world.GetType().Name}][{_world.Id}]"))
                {
                    foreach (Scene scene in _world.Scenes)
                        RecursiveRenderWorld(scene);
                    ImGui.TreePop();
                }
            }
        }

        private static void RecursiveRenderWorld(Entity entity)
        {
            bool destory = false;
            if (ImGui.TreeNode($"[{entity.GetType().Name}][{entity.Id}]({entity.GetHashCode()})"))
            {
                ImGui.PushStyleColor(ImGuiCol.Text, AppUtility.Red);
                if (ImGui.Button("Destory"))
                    destory = true;
                ImGui.PopStyleColor();

                List<EntityComponent> willRemove = null;
                foreach (EntityComponent component in entity.Components)
                {
                    if (ImGui.Button($"x##{component.GetType().Name}"))
                    {
                        if (willRemove == null)
                            willRemove = new List<EntityComponent>();
                        willRemove.Add(component);
                    }
                    ImGui.SameLine();
                    ImGui.TextColored(AppUtility.Green, $"[C]");
                    ImGui.SameLine();
                    ImGui.TextColored(AppUtility.LightBlue, $"[{component.GetType().Name}]");
                }

                var list = new List<Type>(X.Type.GetCollection(typeof(EntityComponent)));
                list.RemoveAll((item) => item == typeof(TransformComponent));
                var listStr = new string[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    listStr[i] = list[i].Name;
                }
                if (ImGui.Button($"+##AddComponent{entity.Id}{_compRef}"))
                {
                    entity.GetOrAddComponent(list[_compRef]);
                }
                ImGui.SameLine();
                ImGui.Combo("##AddComponent", ref _compRef, listStr, listStr.Length);

                if (willRemove != null)
                {
                    foreach (EntityComponent component in willRemove)
                    {
                        entity.RemoveComponent(component);
                    }
                }

                foreach (Entity child in entity.Entities)
                {
                    RecursiveRenderWorld(child);
                }

                var entityList = new List<Type>(X.Type.GetCollection(typeof(Entity)));
                entityList.Insert(0, typeof(Entity));
                entityList.RemoveAll((item) => item == typeof(World) || item == typeof(Scene));
                listStr = new string[entityList.Count];
                for (int i = 0; i < entityList.Count; i++)
                {
                    listStr[i] = entityList[i].Name;
                }
                if (ImGui.Button($"+##AddEntity{entity.Id}{_entityRef}"))
                {
                    entity.AddEntity(entityList[_entityRef]);
                }
                ImGui.SameLine();
                ImGui.Combo("##AddEntity", ref _entityRef, listStr, listStr.Length);

                ImGui.TreePop();
            }
            if (destory)
            {
                if (entity.Parent != null)
                {
                    entity.Parent.RemoveEntity(entity.Id);
                }
            }
        }

        private static async UniTaskVoid LoopSendTo(IConnection connection)
        {
            while (true)
            {
                if (connection.State.Value == ConnectionState.Run)
                {
                    await connection.Send(new StringMessage() { Content = $"{Guid.NewGuid()}" });
                    await Task.Delay(1);
                }
            }
        }

        private static async UniTask TestCreateEntity()
        {
            Scene scene = _world.Scenes.FirstOrDefault();
            scene.AddEntity<Entity>();
            await UniTask.CompletedTask;
        }
    }
}
