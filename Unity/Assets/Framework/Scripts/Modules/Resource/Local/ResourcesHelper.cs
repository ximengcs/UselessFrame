
using System;
using System.IO;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace UselessFrame.ResourceManager
{
    public partial class ResourcesHelper : ResourceHelperBase
    {
        private const string BASE_PATH = "Assets/Resources/";

        protected override T Load<T>(string resPath)
        {
            return (T)Load(typeof(T), resPath);
        }

        protected override object Load(Type type, string resPath)
        {
            return Resources.Load(resPath, type);
        }

        protected override async UniTask<T> LoadAsync<T>(string resPath)
        {
            object asset = Load(typeof(T), resPath);
            return (T)asset;
        }

        protected override async UniTask<object> LoadAsync(Type type, string resPath)
        {
            AsyncState state = new AsyncState(Resources.LoadAsync(resPath, type));
            return await state.Load();
        }

        protected override void Unload(object asset)
        {
            Resources.UnloadAsset((UnityEngine.Object)asset);
        }

        protected override void Unload()
        {
            Resources.UnloadUnusedAssets();
        }

        protected override string GetResPath(string resPath)
        {
            resPath = resPath.Replace(BASE_PATH, string.Empty);
            string extension = Path.GetExtension(resPath);
            if (!string.IsNullOrEmpty(extension))
                resPath = resPath.Replace(extension, string.Empty);
            return resPath;
        }
    }
}
