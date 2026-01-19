
using System;
using YooAsset;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace UselessFrame.ResourceManager
{
    public partial class YooAssetHelper : ResourceHelperBase
    {
        private ResourcePackage _package;
        private ILogger _logger;
        private Dictionary<object, AssetHandle> _loadedAssets;

        public async UniTask InitializeAsync(string packagename, YooAssetMode mode, ILogger logger)
        {
            _logger = logger;
            _loadedAssets = new Dictionary<object, AssetHandle>();
            _package = YooAssets.CreatePackage(packagename);
            YooAssets.Initialize(_logger);

            switch (mode)
            {
                case YooAssetMode.EditorSimulateMode:
                    {
                        await InitPackageByEditorSimulateMode();
                        break;
                    }

                case YooAssetMode.OfflinePlayMode:
                    {
                        await InitPackageByOfflinePlayMode();
                        break;
                    }

                case YooAssetMode.HostPlayMode:
                    {
                        await InitPackageByHostPlayMode();
                        break;
                    }
            }
        }

        private async UniTask InitPackageByEditorSimulateMode()
        {
            var fileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();

            var createParameters = new OfflinePlayModeParameters();
            createParameters.BuildinFileSystemParameters = fileSystemParams;

            var initOperation = _package.InitializeAsync(createParameters);
            await initOperation.Task;

            if (initOperation.Status == EOperationStatus.Succeed)
                _logger.Log("editor resource init successfully！");
            else
                _logger.Error($"editor resource init failure：{initOperation.Error}");

            await RequestPackageVersion();
        }

        private async UniTask InitPackageByOfflinePlayMode()
        {
            var fileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();

            var createParameters = new OfflinePlayModeParameters();
            createParameters.BuildinFileSystemParameters = fileSystemParams;

            var initOperation = _package.InitializeAsync(createParameters);
            await initOperation.Task;

            if (initOperation.Status == EOperationStatus.Succeed)
                _logger.Log("offline resource init successfully！");
            else
                _logger.Error($"offline resource init failure：{initOperation.Error}");

            await RequestPackageVersion();
        }

        private async UniTask InitPackageByHostPlayMode()
        {
            string defaultHostServer = "http://127.0.0.1/CDN/Android/v1.0";
            string fallbackHostServer = "http://127.0.0.1/CDN/Android/v1.0";
            IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
            var cacheFileSystemParams = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
            var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();

            var createParameters = new HostPlayModeParameters();
            createParameters.BuildinFileSystemParameters = buildinFileSystemParams;
            createParameters.CacheFileSystemParameters = cacheFileSystemParams;

            var initOperation = _package.InitializeAsync(createParameters);
            await initOperation.Task;

            if (initOperation.Status == EOperationStatus.Succeed)
                _logger.Log("remote resource init successfully！");
            else
                _logger.Error($"remote resource init failure：{initOperation.Error}");

            await RequestPackageVersion();
        }

        private async UniTask RequestPackageVersion()
        {
            var operation = _package.RequestPackageVersionAsync();
            await operation.Task;

            if (operation.Status == EOperationStatus.Succeed)
            {
                //更新成功
                string packageVersion = operation.PackageVersion;
                _logger.Log($"Request package Version : {packageVersion}");
                await UpdatePackageManifest(packageVersion);
            }
            else
            {
                //更新失败
                _logger.Error(operation.Error);
            }
        }

        private async UniTask UpdatePackageManifest(string packageVersion)
        {
            var operation = _package.UpdatePackageManifestAsync(packageVersion);
            await operation.Task;

            if (operation.Status == EOperationStatus.Succeed)
            {
                //更新成功
                _logger.Log("Update package manifest successfully !");
            }
            else
            {
                //更新失败
                _logger.Error(operation.Error);
            }
        }

        protected override object Load(Type type, string resPath)
        {
            if (!_loadedAssets.TryGetValue(resPath, out AssetHandle handle))
            {
                handle = _package.LoadAssetSync(resPath, type);
                object asset = handle.AssetObject;
                _loadedAssets.Add(asset, handle);
                return asset;
            }
            else
            {
                return handle.AssetObject;
            }
        }

        protected override T Load<T>(string resPath)
        {
            object asset = Load(typeof(T), resPath);
            return (T)asset;
        }

        protected override async UniTask<T> LoadAsync<T>(string resPath)
        {
            object asset = await LoadAsync(typeof(T), resPath);
            return (T)asset;
        }

        protected override async UniTask<object> LoadAsync(Type type, string resPath)
        {
            if (!_loadedAssets.TryGetValue(resPath, out AssetHandle handle))
            {
                handle = _package.LoadAssetAsync(resPath, type);
                await handle.Task;
                object asset = handle.AssetObject;
                _loadedAssets.Add(asset, handle);
                return asset;
            }
            else
            {
                return handle.AssetObject;
            }
        }

        protected override void Unload(object asset)
        {
            if (_loadedAssets.TryGetValue(asset, out AssetHandle handle))
            {
                handle.Release();
                _loadedAssets.Remove(asset);
            }
            _package.TryUnloadUnusedAsset(handle.GetAssetInfo());
        }

        protected override void Unload()
        {
            foreach (var handle in _loadedAssets.Values)
            {
                handle.Release();
            }
            _loadedAssets.Clear();
            _package.UnloadAllAssetsAsync();
        }
    }
}
