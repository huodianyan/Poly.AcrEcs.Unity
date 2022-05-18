#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Poly.ArcEcs.Unity
{
    public sealed class WorldDebugSystem : IEcsSystem
    {
        string worldId;
        GameObject rootGO;
        Transform entityRoot;
        Transform queryRoot;
        Transform archetypeRoot;
        readonly bool _bakeComponentsInName;
        readonly string _entityNameFormat;
        EcsWorld world;
        EntityDebugView[] entityDebugViews;
        Dictionary<int, byte> dirtyEntityDict;
        Dictionary<int, QueryDebugView> queryViewDict;
        Dictionary<int, ArchetypeDebugView> archetypeViewDict;
        Type[] _typesCache;

        public WorldDebugSystem(bool bakeComponentsInName = true, string entityNameFormat = "X8")
        {
            _bakeComponentsInName = bakeComponentsInName;
            _entityNameFormat = entityNameFormat;
        }

        public void Init(EcsWorld world)
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
            dirtyEntityDict = new Dictionary<int, byte>();

            world.EntityCreatedEvent += OnEntityCreated;
            world.EntityDestroyedEvent += OnEntityDestroyed;
            world.WorldResizedEvent += OnWorldResized;
            world.EntityComponentAddedEvent += OnEntityComponentAdded;
            world.EntityComponentAddedEvent += OnEntityComponentAdded;
            world.QueryCreatedEvent += OnQueryCreated;
            world.ArchetypeCreatedEvent += OnArchetypeCreated;

            var entities = Array.Empty<int>();
            var entitiesCount = this.world.GetAllEntities(ref entities);
            for (var i = 0; i < entitiesCount; i++)
            {
                OnEntityCreated(entities[i]);
            }

            queryViewDict = new Dictionary<int, QueryDebugView>();
            foreach(var query in this.world.QueryList)
            {
                OnQueryCreated(query);
            }
            archetypeViewDict = new Dictionary<int, ArchetypeDebugView>();
            foreach(var archetype in this.world.ArchetypeList)
            {
                OnArchetypeCreated(archetype);
            }
        }
        public void Dispose()
        {
            world.EntityCreatedEvent -= OnEntityCreated;
            world.EntityDestroyedEvent -= OnEntityDestroyed;
            world.WorldResizedEvent -= OnWorldResized;
            world.EntityComponentAddedEvent -= OnEntityComponentAdded;
            world.EntityComponentAddedEvent -= OnEntityComponentAdded;
            world.QueryCreatedEvent -= OnQueryCreated;
            world.ArchetypeCreatedEvent -= OnArchetypeCreated;
            Object.Destroy(rootGO);
        }
        public void Update()
        {
            foreach (var pair in dirtyEntityDict)
            {
                var entity = pair.Key;
                var entityName = entity.ToString(_entityNameFormat);
                if (world.GetEntityVersion(entity) > 0)
                {
                    var count = world.GetComponentTypes(entity, ref _typesCache);
                    for (var i = 0; i < count; i++)
                    {
                        entityName = $"{entityName}:{EditorExtensions.GetCleanGenericTypeName(_typesCache[i])}";
                    }
                }
                entityDebugViews[entity].name = entityName;
            }
            dirtyEntityDict.Clear();
        }

        public void OnEntityCreated(int entity)
        {
            if (!entityDebugViews[entity])
            {
                var go = new GameObject();
                go.transform.SetParent(entityRoot, false);
                var entityObserver = go.AddComponent<EntityDebugView>();
                entityObserver.Entity = entity;
                entityObserver.World = world;
                entityObserver.DebugSystem = this;
                entityDebugViews[entity] = entityObserver;
                go.name = entity.ToString(_entityNameFormat);
                // if (_bakeComponentsInName)
                // {
                //     dirtyEntityDict[entity] = 1;
                // }
                // else
                // {
                //     go.name = entity.ToString(_entityNameFormat);
                // }
            }
            entityDebugViews[entity].gameObject.SetActive(true);
        }

        public void OnEntityDestroyed(int entity)
        {
            var entityView = entityDebugViews[entity];
            if (entityView)
            {
                var go = entityView.gameObject;
                go.SetActive(false);
                go.name = entity.ToString(_entityNameFormat);
            }
        }

        public void OnEntityComponentAdded(int entity, int compId)
        {
            if (_bakeComponentsInName)
                dirtyEntityDict[entity] = 1;
        }
        public void OnEntityComponentRemoved(int entity, int compId)
        {
            if (_bakeComponentsInName)
                dirtyEntityDict[entity] = 2;
        }
        public void OnQueryCreated(EcsQuery query)
        {
            if(queryViewDict.TryGetValue(query.Hash, out var queryView))
            {
                Debug.LogError($"QueryView[{query.Hash}] exist!");
                return;
            }
            var go = new GameObject();
            go.transform.SetParent(queryRoot, false);
            queryView = go.AddComponent<QueryDebugView>();
            // queryView.Query = query;
            // queryView.World = world;
            // queryView.DebugSystem = this;
            queryView.Init(world, query, this);
            queryViewDict.Add(query.Hash, queryView);
        }
        public void OnArchetypeCreated(EcsArchetype archetype)
        {
            var archetypeId = archetype.Id;
            if(archetypeViewDict.TryGetValue(archetypeId, out var archetypeView))
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