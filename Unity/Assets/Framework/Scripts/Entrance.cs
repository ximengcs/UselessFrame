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

        public FrameworkSetting Setting => frameworkSetting;

        private void Awake()
        {
            GameObject.DontDestroyOnLoad(this);
        }

        private async void Start()
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
                ValueTuple.Create<Type, object>(typeof(ResourceModule), null),
                ValueTuple.Create(typeof(UIModule), globalCanvas),
            };
            setting.EntranceProcedure = "TestGame.TestProcedure";
            return setting;
        }

        private void InitApplicationSetting()
        {
            int refreshRate = Screen.currentResolution.refreshRate;
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