
using Cysharp.Threading.Tasks;
using System;

namespace UselessFrame.ResourceManager
{
    public partial class YooAssetHelper : ResourceHelperBase
    {
        protected override object Load(Type type, string resPath)
        {
            throw new NotImplementedException();
        }

        protected override T Load<T>(string resPath)
        {
            throw new NotImplementedException();
        }

        protected override UniTask<T> LoadAsync<T>(string resPath)
        {
            throw new NotImplementedException();
        }

        protected override UniTask<object> LoadAsync(Type type, string resPath)
        {
            throw new NotImplementedException();
        }

        protected override void Unload(object asset)
        {
            throw new NotImplementedException();
        }

        protected override void Unload()
        {
            throw new NotImplementedException();
        }
    }
}
