
using UnityEngine;
using UnityXFrame.Core.Diagnotics;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.ECS;

namespace TestGame
{
    [EntityOf(typeof(Entity))]
    public class EntityView : EntityComponent
    {
        private GameObject _inst;

        public GameObject GameObject => _inst;

        protected override void OnInit()
        {
            base.OnInit();
            X.Log.Debug(LogSort.Game, $"entity view init {$"{GetType().Name}-{Entity.Id}"}");
            _inst = new GameObject($"{GetType().Name}-{Entity.Id}");
        }
    }
}
