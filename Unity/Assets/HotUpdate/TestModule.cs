using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityXFrame.Core.Diagnotics;
using UselessFrame.NewRuntime;
using UselessFrame.Runtime;
using UselessFrameUnity.Attributes;

namespace TestGame
{
    [CustomModule]
    public class TestModule : ModuleBase, IModuleUpdater
    {
        protected override void OnInit(object param)
        {
            base.OnInit(param);
            X.Log.Debug(LogSort.Game, $"Test Module OnInit");
        }

        protected override async UniTask OnStart()
        {
            X.Log.Debug(LogSort.Game, $"Test Module OnStart");
            await UniTask.CompletedTask;
        }

        protected override void OnDestroy()
        {
            X.Log.Debug(LogSort.Game, $"Test Module OnDestroy");
        }

        public void OnUpdate(float detalTime)
        {

        }

        public async void Te()
        {
            await UniTask.CompletedTask;
        }
    }
}