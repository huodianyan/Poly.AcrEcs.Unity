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

        private World world;
        private Archetype archetype;
        private WorldDebugSystem system;

        public World World => world;
        public Archetype Archetype => archetype;
        public WorldDebugSystem System => system;

        internal void Init(World world, Archetype archetype, WorldDebugSystem system)
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