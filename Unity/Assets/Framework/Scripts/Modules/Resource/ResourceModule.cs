using UselessFrame.Runtime;
using Cysharp.Threading.Tasks;

namespace UselessFrame.ResourceManager
{
    [Module(typeof(IResourceModule))]
    public class ResourceModule : ModuleBase, IResourceModule
    {
        private IResourceHelper _helper;

        protected override void OnInit(object param)
        {
            base.OnInit(param);
            _helper = (IResourceHelper)param;
        }

        public T Load<T>(string path)
        {
            return _helper.Load<T>(path);
        }

        public UniTask<T> LoadAsync<T>(string path)
        {
            return _helper.LoadAsync<T>(path);
        }
    }
}
