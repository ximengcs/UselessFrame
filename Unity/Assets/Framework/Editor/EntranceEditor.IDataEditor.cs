
using UselessFrameUnity;

public partial class EntranceEditor
{
    private interface IDataEditor
    {
        string Name { get; }
        bool Enable { get; set; }
        void OnInit(FrameworkSetting data);
        void OnUpdate();
        void OnDestroy();
    }

    private abstract class DataEditorBase : IDataEditor
    {
        protected FrameworkSetting m_Data;

        public bool Enable { get; set; }

        public string Name { get; protected set; }

        public virtual void OnDestroy()
        {

        }

        public void OnInit(FrameworkSetting data)
        {
            m_Data = data;
            Name = GetType().Name;
            OnInit();
        }

        public virtual void OnUpdate()
        {

        }

        protected virtual void OnInit()
        {

        }
    }
}
