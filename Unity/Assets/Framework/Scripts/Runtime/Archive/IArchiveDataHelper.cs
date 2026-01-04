
namespace XFrame.Modules.Archives
{
    public interface IArchiveDataHelper
    {
        byte[] ReadBytes(byte[] data);

        byte[] WriteBytes(byte[] buffer);

        string ReadText(byte[] data);

        byte[] WriteText(string text);
    }
}
