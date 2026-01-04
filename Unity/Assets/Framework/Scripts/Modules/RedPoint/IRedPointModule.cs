
namespace UselessFrameUnity.RedPointSystem
{
    public interface IRedPointModule
    {
        IRedPoint GetOrAdd(string path);

        IRedPoint Get(string path);

        IRedPoint Get(string sysName, string path);

        IRedPoint Add(string name);

        IRedPoint Add(string sysName, string name);
    }
}
