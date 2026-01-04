using Core.Application;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using IMGUITestShare.Entities;
using Newtonsoft.Json.Linq;
using TestServer.TestWorld;
using UselessFrame.Net;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.Fiber;
using XFrameServer.Core.Logs;

namespace TestServer
{
    internal class Program
    {
        private static IServer _server;
        internal static Dictionary<long, bool> _testStates;

        static void Main(string[] args)
        {
            Perch.Do();
            _testStates = new Dictionary<long, bool>();
            XSetting setting = new XSetting();
            setting.Loggers = new ILogger[] { new NLogLogger(), new ConsoleLogger() };
            X.Initialize(setting).GetAwaiter().GetResult();
            _server = X.Net.Create(9999, X.Fiber.MainFiber);
            _server.NewConnectionEvent += NewConnectionHandler;
            _server.Start();
            //ShowDebugInfo().Forget();

            GameWorld world = new GameWorld();

            CancellationTokenSource closeTokenSource = new CancellationTokenSource();
            FiberUtility.RunLoopSleep1(X.Update, closeTokenSource.Token, out _);
            Console.WriteLine("Exit");
        }

        private static async UniTask ShowDebugInfo()
        {
            await UniTaskExt.Delay(60 * 60);
            X.Log.Debug(X.GetDebugInfo());
            ShowDebugInfo().Forget();
        }

        private static void NewConnectionHandler(IConnection connection)
        {
            _testStates[connection.Id] = false;
            connection.ReceiveMessageEvent += OnReceiveMessage;
            connection.State.Subscribe(StateChangeHandler);
        }

        private static void StateChangeHandler(IConnection connection, ConnectionState state)
        {
            if (state == ConnectionState.Run)
            {
                
                //Test(connection).Forget();
            }
        }

        private static void OnReceiveMessage(MessageResult message)
        {
            //X.Log.Debug("ServerReceive", message);
            IMessage msg = message.Message;
            if (msg is StringMessage m)
            {
                switch (m.Content)
                {
                    case "1":
                        {
                            JObject node = new JObject();
                            node["Host"] = _server.Host.ToString();
                            node["State"] = _server.State.Value.ToString();

                            var connections = _server.Connections;
                            node["ConnectionCount"] = connections.Count();
                            JArray array = new JArray();
                            foreach (IConnection connection in connections)
                            {
                                JObject obj = new JObject();
                                obj["Id"] = connection.Id;
                                //obj["SendTimes"] = connection.Info.SendTimes;
                                //obj["ReceiveTimes"] = connection.Info.ReceiveTimes;
                                array.Add(obj);
                            }
                            node["Connections"] = array;

                            StringMessage response = NetPoolUtility.CreateMessage<StringMessage>();
                            response.Target = NetPoolUtility.CreateMessage<MessageTarget>();
                            response.Target.Scene = 1;
                            response.Content = node.ToString();
                            message.From.Send(response);
                        }
                        break;
                }
            }
        }

        public static async UniTask Test(IConnection connection)
        {
            X.Log.Debug($"Test Start {connection.Id}");
            try
            {
                if (_testStates[connection.Id])
                    return;
                _testStates[connection.Id] = true;
                while (connection.State.Value == ConnectionState.Run)
                {
                    StringMessage msg = NetPoolUtility.CreateMessage<StringMessage>();
                    msg.Content = $"{Guid.NewGuid()}";
                    connection.Send(msg).Forget();
                    await UniTaskExt.NextFrame();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test expcetion {connection.Id} {ex}");
            }
            _testStates[connection.Id] = false;
            X.Log.Debug($"Test End {connection.Id}");
        }
    }
}
