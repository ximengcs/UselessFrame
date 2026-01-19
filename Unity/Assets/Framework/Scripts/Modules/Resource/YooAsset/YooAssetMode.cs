
namespace UselessFrame.ResourceManager
{
    public enum YooAssetMode
    {
#if UNITY_EDITOR
        EditorSimulateMode,
#endif
        
        OfflinePlayMode,
        HostPlayMode
    }
}
