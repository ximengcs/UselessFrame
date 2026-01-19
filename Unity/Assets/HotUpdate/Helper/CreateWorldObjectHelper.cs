
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityXFrame.Core.Diagnotics;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.ECS;
using UselessFrame.Runtime.Types;

namespace TestGame
{
    public class CreateWorldObjectHelper : IEntityHelper
    {
        private World _world;
        private Dictionary<Type, Type> _compViewMaps;
        private Dictionary<Type, Type> _entityViewMaps;

        public World World => _world;

        public void Bind(World world)
        {
            _compViewMaps = new Dictionary<Type, Type>();
            ITypeCollection types = X.Type.GetCollection(typeof(ComponentOfAttribute));
            foreach (var type in types)
            {
                var attr = (ComponentOfAttribute)X.Type.GetAttribute(type, typeof(ComponentOfAttribute));
                _compViewMaps[attr.Type] = type;
                X.Log.Debug(LogSort.Game, $"OnCreateComponent add {attr.Type.Name} {type.Name}");
            }

            _entityViewMaps = new Dictionary<Type, Type>();
            types = X.Type.GetCollection(typeof(EntityOfAttribute));
            foreach (var type in types)
            {
                var attr = (EntityOfAttribute)X.Type.GetAttribute(type, typeof(EntityOfAttribute));
                _entityViewMaps[attr.Type] = type;
                X.Log.Debug(LogSort.Game, $"OnCreateEntity add {attr.Type.Name} {type.Name}");
            }

            _world = world;
        }

        public void OnCreateEntity(Entity entity)
        {
            X.Log.Debug(LogSort.Game, $"OnCreateEntity {entity.GetType().Name} {entity.Scene == null} {entity.Id} {entity.GetHashCode()}");
            if (_entityViewMaps.TryGetValue(entity.GetType(), out Type viewType))
            {
                X.Log.Debug(LogSort.Game, $"OnCreateEntity add view component1 {viewType.Name}");
                entity.GetOrAddComponent(viewType);
                X.Log.Debug(LogSort.Game, $"OnCreateEntity add view component2 {viewType.Name} {entity.GetHashCode()} {entity.GetComponent<EntityView>()}");
            }
        }

        public void OnCreateComponent(EntityComponent component)
        {
            if (component.Entity.IsDisposed) return;
            X.Log.Debug(LogSort.Game, $"OnCreateComponent {component.Entity.Id} {component.GetType().Name}");
            if (_compViewMaps.TryGetValue(component.GetType(), out Type viewType))
            {
                component.Entity.GetOrAddComponent(viewType);
            }
        }

        public void OnUpdateComponent(EntityComponent component)
        {

        }

        public void OnDestroyComponent(EntityComponent component)
        {
            if (component.Entity.IsDisposed) return;
            X.Log.Debug(LogSort.Game, $"OnDestroyComponent {component.GetHashCode()} {component.Entity.Id} {component.GetType().Name}");
            if (_compViewMaps.TryGetValue(component.GetType(), out Type viewType))
            {
                component.Entity.RemoveComponent(viewType);
            }
        }

        public void OnDestroyEntity(Entity entity)
        {
            X.Log.Debug(LogSort.Game, $"OnDestroyEntity {entity.GetType().Name} {entity.Id}");
        }
    }
}
