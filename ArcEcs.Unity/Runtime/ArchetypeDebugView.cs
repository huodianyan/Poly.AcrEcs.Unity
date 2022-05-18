#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Poly.ArcEcs.Unity
{
    public class ArchetypeDebugView : MonoBehaviour
    {
        [SerializeField]
        public List<EntityDebugView> EntityViewList = new List<EntityDebugView>();

        private EcsWorld world;
        private EcsArchetype archetype;
        private WorldDebugSystem system;

        public EcsWorld World => world;
        public EcsArchetype Archetype => archetype;

        internal void Init(EcsWorld world, EcsArchetype archetype, WorldDebugSystem system)
        {
            this.world = world;
            this.archetype = archetype;
            this.system = system;

            name = $"Archetype[{archetype.Id}]";

            archetype.EntityAddedEvent += OnEntityAdded;
            archetype.EntityRemovedEvent += OnEntityRemoved;
        }
        internal void Release()
        {
            archetype.EntityAddedEvent -= OnEntityAdded;
            archetype.EntityRemovedEvent -= OnEntityRemoved;
        }
        private void OnEntityRemoved(int entity)
        {
            var entityView = system.GetEntityView(entity);
            if(entityView != null)
                EntityViewList.Remove(entityView);
        }
        private void OnEntityAdded(int entity)
        {
            var entityView = system.GetEntityView(entity);
            if(entityView != null)
                EntityViewList.Add(entityView);
        }
    }
}
#endif