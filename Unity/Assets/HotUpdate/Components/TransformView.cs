
using UnityEngine;
using UselessFrame.NewRuntime.ECS;


namespace TestGame
{
    [ComponentOf(typeof(TransformComponent))]
    public class TransformView : EntityComponent
    {
        private EntityView _entityView;

        public Transform Tf => _entityView.GameObject.transform;

        protected override void OnInit()
        {
            base.OnInit();
            _entityView = Entity.GetComponent<EntityView>();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            Debug.LogWarning($"tf view dispose {GetHashCode()}");
        }
    }
}
