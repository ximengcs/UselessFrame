
namespace UselessFrame.NewRuntime.Commands
{
    public interface ICommandManager
    {
        CommandExecuteResult Execute(string cmd);
    }
}
