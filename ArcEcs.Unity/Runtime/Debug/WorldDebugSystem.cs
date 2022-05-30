#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Poly.ArcEcs.Unity
{
    public sealed class WorldDebugSystem : ISystem, IDisposable
    {
        string worldId;
        GameObject rootGO;
        Transform entityRoot;
        Transform queryRoot;
        Transform archetypeRoot;
        readonly bool _bakeComponentsInName;
        readonly string _entityNameFormat;
        World world;
        EntityDebugView[] entityDebugViews;
        // Dictionary<int, byte> dirtyEntityDict;
        List<Entity> dirtyEntityList;
        Dictionary<int, QueryDebugView> queryViewDict;
        Dictionary<int, ArchetypeDebugView> archetypeViewDict;
        Type[] _typesCache;

        public WorldDebugSystem(bool bakeComponentsInName = true, string entityNameFormat = "X8")
        {
            _bakeComponentsInName = bakeComponentsInName;
            _entityNameFormat = entityNameFormat;
        }

        public void Init(World world)
        {
            this.world = world;
            worldId = world.Id;
            rootGO = new GameObject(worldId != null ? $"[ECS-WORLD {worldId}]" : "[ECS-WORLD]");
            Object.DontDestroyOnLoad(rootGO);
            rootGO.hideFlags = HideFlags.NotEditable;
            entityRoot = new GameObject("Entities").transform;
            entityRoot.gameObject.hideFlags = HideFlags.NotEditable;
            entityRoot.SetParent(rootGO.transform, false);
            queryRoot = new GameObject("Queries").transform;
            queryRoot.gameObject.hideFlags = HideFlags.NotEditable;
            queryRoot.SetParent(rootGO.transform, false);
            archetypeRoot = new GameObject("Archetypes").transform;
            archetypeRoot.gameObject.hideFlags = HideFlags.NotEditable;
            archetypeRoot.SetParent(rootGO.transform, false);

            entityDebugViews = new EntityDebugView[this.world.WorldSize];
            // dirtyEntityDict = new Dictionary<int, byte>();
            dirtyEntityList = new List<Entity>();

            world.EntityCreatedEvent += OnEntityCreated;
            world.EntityDestroyedEvent += OnEntityDestroyed;
            world.WorldResizedEvent += OnWorldResized;
            world.ComponentAddedEvent += OnComponentAdded;
            world.ComponentRemovedEvent += OnComponentRemoved;
            world.QueryCreatedEvent += OnQueryCreated;
            world.ArchetypeCreatedEvent += OnArchetypeCreated;

            var entities = Array.Empty<Entity>();
            var entitiesCount = this.world.GetAllEntities(ref entities);
            for (var i = 0; i < entitiesCount; i++)
            {
                OnEntityCreated(entities[i]);
            }

            queryViewDict = new Dictionary<int, QueryDebugView>();
            foreach (var query in this.world.QueryList)
            {
                OnQueryCreated(query);
            }
            archetypeViewDict = new Dictionary<int, ArchetypeDebugView>();
            foreach (var archetype in this.world.ArchetypeList)
            {
                OnArchetypeCreated(archetype);
            }
        }
        public void Dispose()
        {
            world.EntityCreatedEvent -= OnEntityCreated;
            world.EntityDestroyedEvent -= OnEntityDestroyed;
            world.WorldResizedEvent -= OnWorldResized;
            world.ComponentAddedEvent -= OnComponentAdded;
            world.ComponentRemovedEvent -= OnComponentRemoved;
            world.QueryCreatedEvent -= OnQueryCreated;
            world.ArchetypeCreatedEvent -= OnArchetypeCreated;
            Object.Destroy(rootGO);
        }
        public void Update()
        {
            var entityCount = dirtyEntityList.Count;
            if (entityCount > 0)
            {
                for (int i = 0; i < entityCount; i++)
                {
                    var entity = dirtyEntityList[i];
                    entityDebugViews[entity.Index].UpdateName();
                }
                dirtyEntityList.Clear();
            }
        }

        public void OnEntityCreated(Entity entity)
        {
            var entityId = entity.Index;
            var entityView = entityDebugViews[entityId];
            if (!entityView)
            {
                var go = new GameObject();
                go.transform.SetParent(entityRoot, false);
                entityView = go.AddComponent<EntityDebugView>();
                entityDebugViews[entityId] = entityView;
                entityView.Init(world, entity, this);
            }
            else
            {
                entityView.Entity = entity;
                entityView.UpdateName();
                entityView.gameObject.SetActive(true);
            }
        }
        public void OnEntityDestroyed(Entity entity)
        {
            var entityId = entity.Index;
            var entityView = entityDebugViews[entityId];
            if (entityView)
            {
                entityView.Entity = entity;
                entityView.UpdateName();
                entityView.gameObject.SetActive(false);
            }
        }

        public void OnComponentAdded(Entity entity, int compId)
        {
            if (_bakeComponentsInName)
            {
                if (!dirtyEntityList.Contains(entity))
                    dirtyEntityList.Add(entity);
            }
        }
        public void OnComponentRemoved(Entity entity, int compId)
        {
            if (_bakeComponentsInName)
            {
                if (!dirtyEntityList.Contains(entity))
                    dirtyEntityList.Add(entity);
            }
        }
        public void OnQueryCreated(Query query)
        {
            if (queryViewDict.TryGetValue(query.Hash, out var queryView))
            {
                Debug.LogError($"QueryView[{query.Hash}] exist!");
                return;
            }
            var go = new GameObject();
            go.transform.SetParent(queryRoot, false);
            queryView = go.AddComponent<QueryDebugView>();
            queryView.Init(world, query, this);
            queryViewDict.Add(query.Hash, queryView);
        }
        public void OnArchetypeCreated(Archetype archetype)
        {
            var archetypeId = archetype.Id;
            if (archetypeViewDict.TryGetValue(archetypeId, out var archetypeView))
            {
                Debug.LogError($"ArchetypeView[{archetypeId}] exist!");
                return;
            }
            var go = new GameObject();
            go.transform.SetParent(archetypeRoot, false);
            archetypeView = go.AddComponent<ArchetypeDebugView>();
            archetypeView.Init(world, archetype, this);
            archetypeViewDict.Add(archetypeId, archetypeView);
        }
        public void OnWorldResized(int newSize)
        {
            Array.Resize(ref entityDebugViews, newSize);
        }
        public EntityDebugView GetEntityView(int entity)
        {
            return entity >= 0 && entity < entityDebugViews.Length ? entityDebugViews[entity] : null;
        }
        public ArchetypeDebugView GetArchetypeView(int archetypeId)
        {
            archetypeViewDict.TryGetValue(archetypeId, out var archetypeView);
            return archetypeView;
        }
    }
}
#endif