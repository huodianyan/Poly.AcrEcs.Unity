#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Poly.ArcEcs.Unity
{
    public class QueryDebugView : MonoBehaviour
    {
        [SerializeField]
        public List<ArchetypeDebugView> ArchetypeViewList = new List<ArchetypeDebugView>();

        private EcsWorld world;
        private EcsQuery query;
        private WorldDebugSystem system;

        public EcsWorld World => world;
        public EcsQuery Query => query;

        internal void Init(EcsWorld world, EcsQuery query, WorldDebugSystem system)
        {
            this.world = world;
            this.query = query;
            this.system = system;

            name = $"Query[{query.Hash}]";

            query.ArchetypeAddedEvent += OnArchetypeAdded;
        }
        internal void Release()
        {
            query.ArchetypeAddedEvent -= OnArchetypeAdded;
        }
        private void OnArchetypeAdded(EcsArchetype archetype)
        {
            var archetypeView = system.GetArchetypeView(archetype.Id);
            if(archetypeView != null)
                ArchetypeViewList.Add(archetypeView);
        }
    }
}
#endif