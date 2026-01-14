
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

        protected override UniTask<T> LoadAsync<T>(string resPath)
        {
            throw new NotImplementedException();
        }

        protected override UniTask<object> LoadAsync(Type type, string resPath)
        {
            Resources.LoadAsync(resPath, type);
        }

        protected override void Unload<T>(string resPath)
        {
            throw new NotImplementedException();
        }

        protected override void Unload(Type type, string resPath)
        {
            throw new NotImplementedException();
        }

        protected override void Unload()
        {
            throw new NotImplementedException();
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
