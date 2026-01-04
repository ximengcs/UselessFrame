
using DG.Tweening;
using UnityEngine;
using UnityXFrame.Core.Diagnotics;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.ECS;


namespace TestGame
{
    internal class TransformSystem : IAwakeSystem<TransformView>, IUpdateSystem<TransformComponent>
    {
        public void OnAwake(TransformView comp)
        {
            X.Log.Debug(LogSort.Game, $"TransformSystem OnAwake");
        }
        
        public void OnUpdate(TransformComponent oldComp, TransformComponent newComp)
        {
            X.Log.Debug(LogSort.Game, $"TransformSystem OnUpdate {newComp.Entity.Id} {newComp.Position}");
            Transform tf = newComp.Entity.GetComponent<TransformView>().Tf;
            tf.DOLocalMove(new Vector3(newComp.Position.x, newComp.Position.y, newComp.Position.z), 0.2f);
        }
    }
}
