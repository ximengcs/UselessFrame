
namespace XFrame.Modules.Archives
{
    internal class SubJsonArchive : JsonArchiveBase, IJsonArchive
    {
        public IJsonArchive Parent { get; }

        internal SubJsonArchive(IJsonArchive archive, string name)
        {
            Parent = archive;
            Name = $"{archive.Name}_{nameof(SubJsonArchive)}_{name}";
            m_Root = archive.GetOrNewObject(Name);
        }

        public override void ClearData()
        {
            Parent.Remove(Name);
        }
    }
}
