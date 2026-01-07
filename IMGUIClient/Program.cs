using Core.Application;
using Cysharp.Threading.Tasks;
using ImGuiNET;
using System.Net;
using System.Numerics;
using UselessFrame.Net;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.Commands;
using UselessFrame.NewRuntime.Fiber;

namespace TestIMGUIClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Perch.Do();
            X.Initialize(InitFrameSetting()).GetAwaiter().GetResult();

            _app = IApp.Create("Client", 300, 300);
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
        private static int _port = 9999;
        private static List<ClientInfo> _clients = new List<ClientInfo>();
        private static List<ClientInfo> _cache = new List<ClientInfo>();

        private static void OnGUI()
        {
            ImGui.SetNextWindowPos(new Vector2(0, 0));
            ImGui.SetNextWindowSize(new Vector2(300, 300));
            ImGui.Begin("ClientManager", ImGuiWindowFlags.NoResize);
            ImGui.InputInt("Port", ref _port);
            if (ImGui.Button("Connect Remote"))
                AddClient(true);
            if (ImGui.Button("Connect Local"))
                AddClient(false);
            ImGui.End();

            _cache.Clear();
            _cache.AddRange(_clients);
            for (int i = 0; i < _cache.Count; i++)
                RenderClientGUI(_cache[i], i);
        }

        private static void AddClient(bool remote)
        {
            IPEndPoint ip;
            if (remote)
                ip = new IPEndPoint(IPAddress.Parse("8.137.158.164"), _port);
            else
                ip = NetUtility.GetLocalIPEndPoint(_port);
            IConnection client = X.Net.Connect(ip, X.Fiber.MainFiber);
            _clients.Add(new ClientInfo(client, _clients.Count));

            int count = _clients.Count + 1;
            _app.Resize(count >= 4 ? 1200 : count * 300, 300 * (int)Math.Ceiling(count / 4f));
        }

        private static void RemoveClient(ClientInfo info)
        {
            _clients.Remove(info);
            int count = _clients.Count + 1;
            _app.Resize(count >= 4 ? 1200 : count * 300, 300 * (int)Math.Ceiling(count / 4f));
        }

        private static float scrollX = 0.0f; // 当前滚动位置
        private static float color = 0.0f; // 当前滚动位置
        private static void RenderMarqueeText(string text)
        {
            ImGui.ColorConvertHSVtoRGB(color % 360f / 360f, 1f, 1f, out float r, out float g, out float b);
            Vector2 textSize = ImGui.CalcTextSize(text);
            ImGui.BeginChild("Marquee", new Vector2(300, textSize.Y), false, ImGuiWindowFlags.NoScrollbar);
            {
                scrollX += 1;
                color++;
                if (scrollX >= 300) scrollX = 0;
                ImGui.SetScrollX(scrollX);
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(r, g, b, 1));
                ImGui.TextUnformatted(text);
                ImGui.PopStyleColor();
            }
            ImGui.EndChild();
        }

        private static void RenderClientGUI(ClientInfo info, int i)
        {
            IConnection client = info.Client;
            int column = (i + 1) % 4;
            int row = (i + 1) / 4;
            ImGui.SetNextWindowPos(new Vector2(column * 300, row * 300));
            ImGui.SetNextWindowSize(new Vector2(300, 300));
            if (ImGui.Begin($"Client {info.Index}", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.HorizontalScrollbar))
            {
                switch (client.State.Value)
                {
                    case ConnectionState.Run:
                        if (info.ReceiveMsg != null)
                            RenderMarqueeText(info.ReceiveMsg.ToString());
                        ImGui.TextColored(AppUtility.Cyan, $"{client.LocalIP.Address}:{client.LocalIP.Port}");
                        ImGui.TextColored(AppUtility.Magenta, $"{client.RemoteIP.Address}:{client.RemoteIP.Port}");
                        ImGui.TextColored(AppUtility.Green, $"{client.State.Value}");
                        ImGui.TextColored(AppUtility.LightBlue, $"{client.RemoteTime}");
                        ImGui.TextColored(AppUtility.Yellow, $"{info.Latency} ms");
                        ImGui.TextColored(AppUtility.LightGreen, $"{client.Id}");
                        ImGui.BeginGroup();
                        ImGui.InputText("##Message", ref info.SendMsg, 200, ImGuiInputTextFlags.Multiline);
                        ImGui.SameLine();
                        if (ImGui.Button("execute"))
                        {
                            info.ExecuteCommand(new CommandMessage() { CommandStr = info.SendMsg });
                        }
                        if (ImGui.Button("create world"))
                        {
                            info.CreateWorld();
                        }
                        if (ImGui.Button("print world"))
                        {
                            Console.WriteLine(info.World);
                        }
                        info.RenderWorld();

                        if (info.CmdResult.Item1)
                        {
                            CommandExecuteResult cmdRsp = info.CmdResult.Item2;
                            Vector4 col = AppUtility.Green;
                            Vector4 col2 = AppUtility.LightBlue;
                            switch (cmdRsp.Code)
                            {
                                case CommandExecuteCode.ExecuteException:
                                    col = AppUtility.LightRed;
                                    col2 = AppUtility.LightRed;
                                    break;
                                case CommandExecuteCode.FormatError:
                                    col = AppUtility.TransparentRed;
                                    col2 = AppUtility.TransparentRed;
                                    break;
                                case CommandExecuteCode.CommandNotFound:
                                    col = AppUtility.Red;
                                    col2 = AppUtility.Red;
                                    break;
                            }

                            ImGui.TextColored(col, $"{cmdRsp.Code}");
                            ImGui.BeginGroup();
                            ImGui.PushTextWrapPos(0.0f);
                            ImGui.TextColored(col2, $"{cmdRsp.Message}");
                            ImGui.PopTextWrapPos();
                            ImGui.EndGroup();
                        }

                        ImGui.EndGroup();
                        if (!info.Testing)
                        {
                            if (ImGui.Button("loop send test"))
                                info.Test().Forget();
                        }
                        break;

                    case ConnectionState.Connect:
                    case ConnectionState.CheckConnect:
                    case ConnectionState.TokenVerify:
                    case ConnectionState.TokenResponse:
                        ImGui.TextColored(AppUtility.LightBlue, $"[{client.State.Value}]");
                        break;

                    case ConnectionState.CloseRequest:
                    case ConnectionState.CloseResponse:
                    case ConnectionState.Dispose:
                        ImGui.TextColored(AppUtility.Yellow, $"server may not start.");
                        //if (ImGui.Button("Reconnect"))
                        //    client.Reconnect();
                        break;

                    default: ImGui.TextColored(AppUtility.Red, $"[{client.State.Value}]"); break;
                }

                switch (client.State.Value)
                {
                    case ConnectionState.CloseResponse:
                    case ConnectionState.CloseRequest:
                    case ConnectionState.Dispose:
                        ImGui.PushStyleColor(ImGuiCol.Text, AppUtility.LightRed);
                        if (ImGui.Button("Remove Client"))
                            RemoveClient(info);
                        ImGui.PopStyleColor();
                        break;

                    default:
                        ImGui.PushStyleColor(ImGuiCol.Text, AppUtility.Red);
                        if (ImGui.Button("Close Connect"))
                            client.Close();
                        ImGui.PopStyleColor();
                        break;
                }

                ImGui.End();
            }
        }
    }
}
