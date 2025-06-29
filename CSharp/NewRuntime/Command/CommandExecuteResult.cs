
using UselessFrame.Net;

namespace UselessFrame.NewRuntime.Commands
{
    public struct CommandExecuteResult
    {
        public CommandExecuteCode Code;
        public string Message;

        public CommandExecuteResult(CommandExecuteCode code, string message)
        {
            Code = code;
            Message = message;
        }

        public CommandExecuteResult(CommandExecuteCode code)
        {
            Code = code;
            Message = string.Empty;
        }

        public CommandExecuteResult(MessageResult msg)
        {
            CommandResponseMessage rspMsg = (CommandResponseMessage)msg.Message;
            Code = (CommandExecuteCode)rspMsg.CommandCode;
            Message = rspMsg.CommandResult;
        }

        public override string ToString()
        {
            return $"execute result code : {Code}\n{Message}";
        }
    }
}
