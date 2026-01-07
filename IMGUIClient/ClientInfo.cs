using Core.Application;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using ImGuiNET;
using Newtonsoft.Json;
using UselessFrame.Net;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.Commands;
using UselessFrame.NewRuntime.ECS;

namespace TestIMGUIClient
{
    public class ClientInfo
    {
        public IConnection Client;
        public string SendMsg = string.Empty;
        public IMessage ReceiveMsg;
        public int Index;
        public bool Testing;
        public ValueTuple<bool, CommandExecuteResult> CmdResult;
        private Queue<CommandMessage> CmdQueue;
        public long Latency;
        public World World;

        public ClientInfo(IConnection client, int index)
        {
            Client = client;
            SendMsg = string.Empty;
            Index = index;
            CmdQueue = new Queue<CommandMessage>();
            Client.ReceiveMessageEvent += ReceiveMessage;
            client.State.Subscribe(StateChange, true);
        }

        public void CreateWorld()
        {
            World = X.World.Create(WorldSetting.Client(9999));
        }

        public void ExecuteCommand(CommandMessage cmd)
        {
            if (CmdQueue.Count <= 0)
            {
                CmdQueue.Enqueue(cmd);
                ExecuteRemoteCommand().Forget();
            }
            else
            {
                CmdQueue.Enqueue(cmd);
            }
        }

        public void RenderWorld()
        {
            if (World != null)
            {
                if (ImGui.TreeNode($"[{World.GetType().Name}][{World.Id}]"))
                {
                    foreach (Scene scene in World.Scenes)
                        RecursiveRenderWorld(scene);
                    ImGui.TreePop();
                }
            }
        }

        private void RecursiveRenderWorld(Entity entity)
        {
            if (ImGui.TreeNode($"[{entity.GetType().Name}][{entity.Id}]"))
            {
                foreach (EntityComponent component in entity.Components)
                {
                    ImGui.TextColored(AppUtility.Green, $"[C]");
                    ImGui.SameLine();
                    ImGui.TextColored(AppUtility.LightBlue, $"[{component.GetType().Name}]");
                }
                foreach (Entity child in entity.Entities)
                {
                    RecursiveRenderWorld(child);
                }
                ImGui.TreePop();
            }
        }
        private async UniTask ExecuteRemoteCommand()
        {
            if (CmdQueue.TryDequeue(out CommandMessage cmdInfo))
            {
                MessageResult msgResult = await Client.SendWait(cmdInfo);
                CommandExecuteResult execResult = new CommandExecuteResult(msgResult);
                CmdResult = ValueTuple.Create(true, execResult);
            }
        }

        private void StateChange(ConnectionState state)
        {
            if (state == ConnectionState.Run)
                TestLatency().Forget();
            //X.SystemLog.Debug($"State Change {state}");
        }

        private void ReceiveMessage(MessageResult msg)
        {
            ReceiveMsg = msg.Message;
            if (ReceiveMsg is StringMessage m)
            {
                if (m.Target != null)
                {
                    if (m.Target.Scene == 1)
                    {
                        X.Log.Debug($"\n{JsonConvert.DeserializeObject(m.Content)}");
                    }
                }
            }
        }

        public async UniTask TestLatency()
        {
            LatencyResult result = await Client.TestLatency();
            Latency = result.DeltaMillTime;
            await UniTaskExt.Delay(1);
            TestLatency().Forget();
        }

        public async UniTask Test()
        {
            X.Log.Debug($"Test Start {Client.Id}");
            try
            {
                if (Testing)
                    return;
                Testing = true;
                while (Client.State.Value == ConnectionState.Run)
                {
                    await Client.Send(new StringMessage() { Content = $"{Guid.NewGuid()}" });
                    await UniTaskExt.Delay(1 / 1000f);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test expcetion {Client.Id} {ex}");
            }
            Testing = false;
            X.Log.Debug($"Test End {Client.Id}");
        }
    }
}
