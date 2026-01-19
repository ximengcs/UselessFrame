using System;
using UnityEngine;
using UnityXFrame.Core;
using UnityXFrame.Core.Diagnotics;
using UselessFrame.NewRuntime;
using UselessFrame.ResourceManager;
using UselessFrame.Runtime;
using UselessFrame.UIElements;
using UselessFrameUnity.Attributes;

namespace UselessFrameUnity
{
    public class Entrance : SingletonMono<Entrance>
    {
        [SerializeField]
        private Canvas globalCanvas;

        [SerializeField]
        private FrameworkSetting frameworkSetting;

        private IResourceHelper _resourcesHelper;
        private IResourceHelper _remoteResourceHelper;

        public FrameworkSetting Setting => frameworkSetting;

        private void Awake()
        {
            GameObject.DontDestroyOnLoad(this);
        }

        private async void Start()
        {
            _resourcesHelper = new ResourcesHelper();
            _remoteResourceHelper = new YooAssetHelper();

            InitFramework();
        }

        private async void InitFramework()
        {
            InitApplicationSetting();
            await X.Initialize(InitFrameSetting());
            X.Module.AddHandler<UpdateHandler>();
        }

        private XSetting InitFrameSetting()
        {
            XSetting setting = new XSetting();
            setting.Log = new UnityLogManager(null);
            setting.TypeFilter = new TypeFilter();
            setting.Loggers = new[] { InitLogColorSetting() };
            setting.ModuleAttributes = new[]
            {
                typeof(FrameModuleAttribute),
                typeof(CustomModuleAttribute),
            };
            setting.Modules = new[]
            {
                ValueTuple.Create<Type, object, int>(typeof(ResourceModule), _resourcesHelper, C.LOCAL_ID),
                ValueTuple.Create<Type, object, int>(typeof(ResourceModule), _remoteResourceHelper, C.COMMON_ID),
                ValueTuple.Create(typeof(UIModule), globalCanvas, C.DEFAULT_ID),
            };
            setting.EntranceProcedure = "TestGame.TestProcedure";
            return setting;
        }

        private void InitApplicationSetting()
        {
            int refreshRate = (int)Math.Round(Screen.currentResolution.refreshRateRatio.value);
            Application.targetFrameRate = refreshRate;
            QualitySettings.vSyncCount = 1;
        }

        private UnityLogger InitLogColorSetting()
        {
            UnityLogger logger = new UnityLogger();
            foreach (DebugColor colorData in frameworkSetting.LogMark)
            {
                if (colorData.Value)
                    logger.Register(colorData.Key, colorData.Color);
            }
            return logger;
        }


        private void Update()
        {
            X.Update(Time.deltaTime);
        }

        private void OnDestroy()
        {
            X.Shutdown();
        }
    }
}