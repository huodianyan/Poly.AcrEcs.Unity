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

        private World world;
        private Query query;
        private WorldDebugSystem system;

        public World World => world;
        public Query Query => query;

        internal void Init(World world, Query query, WorldDebugSystem system)
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
        private void OnArchetypeAdded(Archetype archetype)
        {
            var archetypeView = system.GetArchetypeView(archetype.Id);
            if(archetypeView != null)
                ArchetypeViewList.Add(archetypeView);
        }
    }
}
#endif