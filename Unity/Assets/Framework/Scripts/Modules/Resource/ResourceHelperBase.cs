
using System;
using Cysharp.Threading.Tasks;

namespace UselessFrame.ResourceManager
{
    public abstract class ResourceHelperBase : IResourceHelper
    {
        protected IResourceHelper _toResHelper;
        protected IResourceHelper _fromResHelper;

        void IResourceHelper.SetResourceHelper(IResourceHelper helper)
        {
            _toResHelper = helper;
        }

        internal void OnAttachResHelper(IResourceHelper helper)
        {
            _fromResHelper = helper;
        }

        object IResourceHelper.Load(Type type, string resPath)
        {
            resPath = GetResPath(resPath);
            if (_toResHelper != null)
                return _toResHelper.Load(type, resPath);
            else
                return Load(type, resPath);
        }

        protected abstract object Load(Type type, string resPath);

        T IResourceHelper.Load<T>(string resPath)
        {
            resPath = GetResPath(resPath);
            if (_toResHelper != null)
                return _toResHelper.Load<T>(resPath);
            else
                return Load<T>(resPath);
        }

        protected abstract T Load<T>(string resPath);

        UniTask<T> IResourceHelper.LoadAsync<T>(string resPath)
        {
            resPath = GetResPath(resPath);
            if (_toResHelper != null)
                return _toResHelper.LoadAsync<T>(resPath);
            else
                return LoadAsync<T>(resPath);
        }

        protected abstract UniTask<T> LoadAsync<T>(string resPath);

        UniTask<object> IResourceHelper.LoadAsync(Type type, string resPath)
        {
            resPath = GetResPath(resPath);
            if (_toResHelper != null)
                return _toResHelper.LoadAsync(type, resPath);
            else
                return LoadAsync(type, resPath);
        }

        protected abstract UniTask<object> LoadAsync(Type type, string resPath);

        void IResourceHelper.Unload<T>(string resPath)
        {
            resPath = GetResPath(resPath);
            if (_toResHelper != null)
                _toResHelper.Unload<T>(resPath);
            else
                Unload<T>(resPath);
        }

        protected abstract void Unload<T>(string resPath);

        void IResourceHelper.Unload(Type type, string resPath)
        {
            resPath = GetResPath(resPath);
            if (_toResHelper != null)
                _toResHelper.Unload(type, resPath);
            else
                Unload(type, resPath);
        }

        protected abstract void Unload(Type type, string resPath);

        void IResourceHelper.Unload()
        {
            if (_toResHelper != null)
                _toResHelper.Unload();
            else
                Unload();
        }

        protected abstract void Unload();

        protected virtual string GetResPath(string resPath)
        {
            return resPath;
        }
    }
}
