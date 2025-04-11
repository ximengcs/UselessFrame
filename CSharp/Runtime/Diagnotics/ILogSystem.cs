
namespace UselessFrame.Runtime.Diagnotics
{
    public interface ILogSystem : ILogger
    {
        bool Power { get; set; }

        void AddLogger<T>() where T : ILogger;

        void RemoveLogger<T>() where T : ILogger;
    }
}
