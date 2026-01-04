
using Cysharp.Threading.Tasks;
using UselessFrame.Net;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.Commands;

namespace TestServer.Commands
{
    [CommandClass]
    public class TestServerCommandClass
    {
        [Command("log-keepalive")]
        public void SetKeepaliveLog(CommandHandle handle,
            [CommandOption("--server_id")] long serverIdStr,
            [CommandOption("--client_id")] long clientIdStr,
            [CommandArgument("--value")] bool show)
        {
            IServer server = X.Net.GetServer(serverIdStr);
            IConnection connection = server.GetConnection(clientIdStr);
            if (server != null)
            {
                ConnectionSetting setting = connection.GetRuntimeData<ConnectionSetting>();
                setting.ShowReceiveKeepaliveLog = show;

                CommandExecuteResult result = new CommandExecuteResult(CommandExecuteCode.OK);
                result.Message = $"execute log-keepalive, set ShowReceiveKeepaliveLog -> {show}";
                handle.Result = result;
                X.Log.Debug(result.Message);
            }
            else
            {
                X.Log.Error($"execute log-keepalive error, {serverIdStr}, {clientIdStr}");
            }
        }

        [Command("log-receive")]
        public void SetReceiveMessageLog(CommandHandle handle,
            [CommandOption("--server_id")] long serverIdStr,
            [CommandOption("--client_id")] long clientIdStr,
            [CommandArgument("--value")] bool show)
        {
            IServer server = X.Net.GetServer(serverIdStr);
            IConnection connection = server.GetConnection(clientIdStr);
            if (server != null)
            {
                ConnectionSetting setting = connection.GetRuntimeData<ConnectionSetting>();
                setting.ShowReceiveMessageInfo = show;

                CommandExecuteResult result = new CommandExecuteResult(CommandExecuteCode.OK);
                result.Message = $"execute log-receive, set ShowReceiveMessageInfo -> {show}";
                handle.Result = result;
                X.Log.Debug(result.Message);
            }
            else
            {
                X.Log.Error($"execute log-receive error, {serverIdStr}, {clientIdStr}");
            }
        }

        [Command("log-send")]
        public void SetSendMessageLog(CommandHandle handle,
            [CommandOption("--server_id")] long serverIdStr,
            [CommandOption("--client_id")] long clientIdStr,
            [CommandArgument("--value")] bool show)
        {
            IServer server = X.Net.GetServer(serverIdStr);
            IConnection connection = server.GetConnection(clientIdStr);
            if (server != null)
            {
                ConnectionSetting setting = connection.GetRuntimeData<ConnectionSetting>();
                setting.ShowSendMessageInfo = show;

                CommandExecuteResult result = new CommandExecuteResult(CommandExecuteCode.OK);
                result.Message = $"execute log-send, set ShowSendMessageInfo -> {show}";
                handle.Result = result;
                X.Log.Debug(result.Message);
            }
            else
            {
                X.Log.Error($"execute log-send error, {serverIdStr}, {clientIdStr}");
            }
        }

        [Command("loop-send-state")]
        public void CheckLoopSendState(CommandHandle handle,
            [CommandOption("--server_id")] long serverIdStr,
            [CommandOption("--client_id")] long clientIdStr)
        {
            IServer server = X.Net.GetServer(serverIdStr);
            IConnection connection = server.GetConnection(clientIdStr);
            if (server != null)
            {
                CommandExecuteResult result = new CommandExecuteResult(CommandExecuteCode.OK);
                result.Message = $"execute loop-send-state, state is [{Program._testStates[connection.Id]}]";
                handle.Result = result;
                X.Log.Debug(result.Message);
            }
            else
            {
                X.Log.Error($"execute log-send error, {serverIdStr}, {clientIdStr}");
            }
        }

        [Command("loop-send")]
        public void TestLoopSend(CommandHandle handle,
            [CommandOption("--server_id")] long serverIdStr,
            [CommandOption("--client_id")] long clientIdStr)
        {
            IServer server = X.Net.GetServer(serverIdStr);
            IConnection connection = server.GetConnection(clientIdStr);
            if (server != null)
            {
                Program.Test(connection).Forget();
                CommandExecuteResult result = new CommandExecuteResult(CommandExecuteCode.OK);
                result.Message = $"execute loop-send";
                handle.Result = result;
                X.Log.Debug(result.Message);
            }
            else
            {
                X.Log.Error($"execute log-send error, {serverIdStr}, {clientIdStr}");
            }
        }
    }
}
