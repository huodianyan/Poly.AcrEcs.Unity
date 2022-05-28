using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Poly.ArcEcs
{
    #region EcsWorld
    public class EcsWorld : IDisposable
    {
        //World
        private readonly string id;
        private readonly object shared;
        private bool isInited;
        private bool disposed = false;
        //Entity
        internal EcsEntityInternal[] entityInternals;
        internal int entityCount;
        private int[] recycledEntities;
        private int recycledEntityCount;
        //ArcheType
        private byte[] tempCompIds;
        //internal EcsArchetype[] archetypes;
        //internal int archetypeCount;
        internal List<EcsArchetype> archetypeList;
        //Component
        internal EcsComponentType[] compTypes;
        internal byte compTypeCount;
        private Dictionary<Type, EcsComponentType> compTypeDict;
        //Query
        //internal EcsQuery[] queries;
        //internal int queryCount;
        internal List<EcsQuery> queryList;
        //internal EcsQueryDesc[] queryDescs;
        private Dictionary<int, EcsQuery> queryDict;
        private EcsQueryDesc[] recycledQueryDescs;
        private int recycledQueryDescCount;
        //System
        private Dictionary<Type, IEcsSystem> systemDict;
        private List<IEcsSystem> systemList;
        //private IEcsRunSystem[] runSystems;
        //private int runSystemCount;

        public string Id => id;
        public bool IsDisposed => disposed;
        public int WorldSize => entityInternals.Length;
        public int EntityCount => entityCount - recycledEntityCount;
        public int ArchetypeCount => archetypeList.Count;
        public IList<EcsArchetype> ArchetypeList => archetypeList;
        public IList<EcsQuery> QueryList => queryList;
        public int QueryCount => queryList.Count;
        public IList<IEcsSystem> SystemList => systemList;

        public event Action<int> WorldResizedEvent;
        public event Action<EcsEntity> EntityCreatedEvent;
        public event Action<EcsEntity> EntityDestroyedEvent;
        public event Action<EcsEntity, int> ComponentAddedEvent;
        public event Action<EcsEntity, int> ComponentRemovedEvent;
        public event Action<EcsQuery> QueryCreatedEvent;
        public event Action<EcsArchetype> ArchetypeCreatedEvent;

        public EcsWorld(string id = "Default", object shared = null, in Config cfg = default)
        {
            //World
            this.id = id;
            this.shared = shared;
            //Entity
            var capacity = cfg.EntityCapacity > 0 ? cfg.EntityCapacity : Config.EntityCapacityDefault;
            entityInternals = new EcsEntityInternal[capacity];
            entityCount = 0;
            capacity = cfg.RecycledEntityCapacity > 0 ? cfg.RecycledEntityCapacity : Config.RecycledEntityCapacityDefault;
            recycledEntities = new int[capacity];
            recycledEntityCount = 0;
            //ArcheType
            //capacity = cfg.ArchetypeCapacity > 0 ? cfg.ArchetypeCapacity : Config.ArchetypeCapacityDefault;
            var emptyArchetype = new EcsArchetype(this, 0, null, 0);
            //archetypes = new EcsArchetype[capacity];
            //archetypeCount = 0;
            //archetypes[archetypeCount++] = emptyArchetypeData;
            archetypeList = new List<EcsArchetype>();
            archetypeList.Add(emptyArchetype);

            //Component
            //compTypeDatas = new FastArray<EcsComponentTypeData>(32);
            tempCompIds = new byte[256];
            compTypes = new EcsComponentType[256];
            compTypeCount = 0;
            compTypeDict = new Dictionary<Type, EcsComponentType>();
            //Query
            //capacity = cfg.QueryCapacity > 0 ? cfg.QueryCapacity : Config.QueryCapacityDefault;
            //queries = new EcsQuery[capacity];
            //queryCount = 0;
            queryList = new List<EcsQuery>();
            //queryDescs = new EcsQueryDesc[capacity];
            queryDict = new Dictionary<int, EcsQuery>();
            recycledQueryDescs = new EcsQueryDesc[32];
            recycledQueryDescCount = 0;
            //System
            //capacity = cfg.SystemCapacity > 0 ? cfg.SystemCapacity : Config.SystemCapacityDefault;
            systemDict = new Dictionary<Type, IEcsSystem>();
            systemList = new List<IEcsSystem>();
            //runSystems = new IEcsRunSystem[capacity];
            //runSystemCount = 0;
        }
        ~EcsWorld()
        {
            Dispose(disposing: false);
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                // Free any other managed objects here.
                //Archetype
                //for (int i = 0; i < archetypeCount; i++)
                //{
                //    ref var archetype = ref archetypes[i];
                //    archetype.Dispose();
                //}
                //archetypes = null;
                //archetypeCount = 0;
                for (int i = 0; i < archetypeList.Count; i++)
                {
                    var archetype = archetypeList[i];
                    archetype.Dispose();
                }
                archetypeList.Clear();
                //Entity
                entityInternals = null;
                entityCount = 0;
                recycledEntities = null;
                recycledEntityCount = 0;
                //Component
                tempCompIds = null;
                compTypes = null;
                compTypeCount = 0;
                compTypeDict.Clear();
                compTypeDict = null;
                //Query
                //queries = null;
                //queryCount = 0;
                queryList.Clear();
                queryList = null;
                //queryDescs = null;
                queryDict.Clear();
                queryDict = null;
                recycledQueryDescs = null;
                recycledQueryDescCount = 0;
                //System
                systemDict.Clear();
                systemDict = null;
                systemList.Clear();
                systemList = null;
            }
            // Free any unmanaged objects here.
            disposed = true;
        }
        public void Init()
        {
            foreach (var system in systemList)
                system.Init(this);
            isInited = true;
        }
        public void Update()
        {
            var count = systemList.Count;
            for (int i = 0; i < count; i++)
                systemList[i].Update();
        }
        public T GetShared<T>()
        {
            return (T)shared;
        }

        #region Entity
        public int GetAllEntities(ref EcsEntity[] entities)
        {
            var count = entityCount - recycledEntityCount;
            if (entities == null || entities.Length < count)
                entities = new EcsEntity[count];
            var id = 0;
            for (int i = 0, iMax = entityCount; i < iMax; i++)
            {
                ref var entityInternal = ref this.entityInternals[i];
                // should we skip empty entities here?
                if (entityInternal.Version > 0 && entityInternal.ComponentCount >= 0)
                    entities[id++] = entityInternal;
            }
            return count;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEntityValid(EcsEntity entity) => disposed ? false : entityInternals[entity.Index].Version == entity.Version;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EcsEntity GetEntity(int entityId) => disposed ? default : entityInternals[entityId];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool IsEntityValid(int entityId) => entityId >= 0 && entityId < entityCount && entityInternals[entityId].Version > 0;

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal ref readonly EcsEntityInternal GetEntityInternal(int entityId) => ref entities[entityId];
        //public short GetEntityVersion(int entityId) => entities[entityId].Version;
        private ref EcsEntityInternal CreateEntityInternal()
        {
            int entityId;
            if (recycledEntityCount > 0)
            {
                entityId = recycledEntities[--recycledEntityCount];
                ref var entity = ref entityInternals[entityId];
                entity.Version = (short)-entity.Version;
            }
            else
            {
                // new entity.
                if (entityCount == entityInternals.Length)
                {
                    var newSize = entityCount << 1;
                    Array.Resize(ref entityInternals, newSize);
                    WorldResizedEvent?.Invoke(newSize);
                }
                entityId = entityCount++;
                ref var entity = ref entityInternals[entityId];
                entity.Index = entityId;
                entity.Version = 1;
            }
            //EntityCreatedEvent?.Invoke(entity);
            return ref entityInternals[entityId];
        }
        private void DestroyEntityInternal(ref EcsEntityInternal entityInternal)
        {
            var entityId = entityInternal.Index;
            entityInternal.Version = (short)(entityInternal.Version == short.MaxValue ? -1 : -(entityInternal.Version + 1));
            if (recycledEntityCount == recycledEntities.Length) Array.Resize(ref recycledEntities, recycledEntityCount << 1);
            entityInternal.ComponentCount = 0;
            entityInternal.ArchetypeId = 0;
            entityInternal.ArchetypeChunkId = 0;
            recycledEntities[recycledEntityCount++] = entityId;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EcsEntity CreateEntity() => CreateEntityInternal(null, 0);
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public EcsEntity CreateEntity(params Type[] types) => CreateEntityInternal(GetCompnentIds(types), types.Length);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EcsEntity CreateEntity(params byte[] compIds) => CreateEntityInternal(compIds);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref EcsEntityInternal CreateEntityInternal(params byte[] compIds)
        {
            if (compIds.Length > 1) Array.Sort(compIds);
            return ref CreateEntityInternal(compIds, compIds.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref EcsEntityInternal CreateEntityInternal(byte[] compIds, int count)
        {
            ref var entity = ref CreateEntityInternal();
            //ref var archetype = ref GetArchetype(compIds, count);
            var archetype = GetArchetype(compIds, count);
            entity.ArchetypeId = archetype.id;
            var entityId = entity.Index;
            archetype.AddEntity(ref entity);
            //Console.WriteLine($"EcsWorld.CreateEntity: {entityId},{archetypeData.entityList.Count}");
            EntityCreatedEvent?.Invoke(entity);
            return ref entity;
        }
        public void DestroyEntity(EcsEntity entity)
        {
            if (!IsEntityValid(entity)) return;
            ref var entityInternal = ref entityInternals[entity.Index];
            if (entityInternal.Version < 0) return;
            var archetypeId = entityInternal.ArchetypeId;
            //ref var archetype = ref archetypes[archetypeId];
            var archetype = archetypeList[archetypeId];
            archetype.RemoveEntity(ref entityInternal);

            DestroyEntityInternal(ref entityInternal);
            //entity.Version = (short)(entity.Version == short.MaxValue ? -1 : -(entity.Version + 1));
            //if (recycledEntityCount == recycledEntities.Length) Array.Resize(ref recycledEntities, recycledEntityCount << 1);
            //recycledEntities[recycledEntityCount++] = entityId;
            EntityDestroyedEvent?.Invoke(entityInternal);
        }

        public bool HasComponent<T>(EcsEntity entity) where T : struct
        {
            ref var entityInternal = ref entityInternals[entity.Index];
            if (entity.Version != entityInternal.Version) return false;
            var compId = GetComponentId<T>();
            return HasComponentInternal(ref entityInternal, compId);
        }
        public bool HasComponent(EcsEntity entity, byte compId)
        {
            ref var entityInternal = ref entityInternals[entity.Index];
            if (entity.Version != entityInternal.Version) return false;
            return HasComponentInternal(ref entityInternal, compId);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool HasComponentInternal(ref EcsEntityInternal entityInternal, byte compId) => archetypeList[entityInternal.ArchetypeId].HasComponent(compId);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetComponent<T>(EcsEntity entity) where T : struct
        {
            ref var entityInternal = ref entityInternals[entity.Index];
            if (entity.Version != entityInternal.Version) return ref GetDefaultComp<T>();
            var compId = GetComponentId<T>();
            return ref GetComponentInternal<T>(ref entityInternal, compId);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetComponent<T>(EcsEntity entity, byte compId) where T : struct
        {
            ref var entityInternal = ref entityInternals[entity.Index];
            if (entity.Version != entityInternal.Version) return ref GetDefaultComp<T>();
            return ref GetComponentInternal<T>(ref entityInternal, compId);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref T GetComponentInternal<T>(ref EcsEntityInternal entityInternal, byte compId) where T : struct
        {
            var archetype = archetypeList[entityInternal.ArchetypeId];
            return ref archetype.GetComponent<T>(compId, entityInternal.ArchetypeChunkId);
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public ref T AddComponent<T>(int entityId) where T : struct
        //{
        //    var addCompId = GetComponentId(typeof(T));
        //    return ref AddComponent<T>(entityId, addCompId);
        //}
        //public ref T AddComponent<T>(int entityId, byte addCompId) where T : struct
        //{
        //    ref var entity = ref entities[entityId];
        //    //ref var archetype = ref archetypes[entity.ArchetypeId];
        //    var archetype = archetypeList[entity.ArchetypeId];
        //    //ref var nextArchetype = ref GetNextArchetype(ref archetype, addCompId);
        //    var nextArchetype = GetNextArchetype(archetype, addCompId);
        //    //move comp data
        //    var chunkId = entity.ArchetypeChunkId;
        //    foreach (var compId in archetype.compIds)
        //    {
        //        ref var compType = ref GetComponentTypeData(compId);
        //        compType.CopyChunkComponent(archetype, chunkId, nextArchetype);
        //        //var comp = archetypeData.GetComponent<T>(compId, chunkId);
        //        //nextArchetypeData.AddComponent(compId, comp);
        //    }
        //    ref var comp = ref nextArchetype.AddComponent<T>(addCompId);
        //    archetype.RemoveEntity(ref entity);
        //    nextArchetype.AddEntity(ref entity);
        //    ComponentAddedEvent?.Invoke(entityId, addCompId);
        //    return ref comp;
        //}
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddComponent<T>(EcsEntity entity, T addComp = default) where T : struct
        {
            ref var entityInternal = ref entityInternals[entity.Index];
            if (entity.Version != entityInternal.Version) return;
            var addCompId = GetComponentId<T>();
            AddComponentInternal<T>(ref entityInternal, addComp, addCompId);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddComponent<T>(EcsEntity entity, T addComp, byte addCompId) where T : struct
        {
            ref var entityInternal = ref entityInternals[entity.Index];
            if (entity.Version != entityInternal.Version) return;
            AddComponentInternal<T>(ref entityInternal, addComp, addCompId);
        }
        internal void AddComponentInternal<T>(ref EcsEntityInternal entityInternal, T addComp, byte addCompId) where T : struct
        {
            //ref var entity = ref entities[entityId];
            //ref var archetype = ref archetypes[entity.ArchetypeId];
            var archetype = archetypeList[entityInternal.ArchetypeId];
            //ref var nextArchetype = ref GetNextArchetype(ref archetype, addCompId);
            var nextArchetype = GetNextArchetype(archetype, addCompId);
            //move comp data
            var chunkId = entityInternal.ArchetypeChunkId;
            foreach (var compId in archetype.compIds)
            {
                ref var compType = ref GetComponentTypeData(compId);
                compType.CopyChunkComponent(archetype, chunkId, nextArchetype);
                //var comp = archetypeData.GetComponent<T>(compId, chunkId);
                //nextArchetypeData.AddComponent(compId, comp);
            }
            nextArchetype.AddComponent(addCompId, addComp);
            archetype.RemoveEntity(ref entityInternal);
            nextArchetype.AddEntity(ref entityInternal);
            entityInternal.ComponentCount++;
            ComponentAddedEvent?.Invoke(entityInternal, addCompId);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveComponent(EcsEntity entity, byte removeCompId)
        {
            ref var entityInternal = ref entityInternals[entity.Index];
            if (entity.Version != entityInternal.Version) return;
            RemoveComponentInternal(ref entityInternal, removeCompId);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveComponent<T>(EcsEntity entity) where T : struct
        {
            ref var entityInternal = ref entityInternals[entity.Index];
            if (entity.Version != entityInternal.Version) return;
            var removeCompId = GetComponentId<T>();
            //RemoveComponent(entityId, removeCompId);
            RemoveComponentInternal(ref entityInternal, removeCompId);
        }
        private void RemoveComponentInternal(ref EcsEntityInternal entityInternal, byte removeCompId)
        {
            //ref var entity = ref entities[entityId];
            //ref var archetype = ref archetypes[entity.ArchetypeId];
            var archetype = archetypeList[entityInternal.ArchetypeId];
            //ref var preArchetype = ref GetPreArchetype(ref archetype, removeCompId);
            var preArchetype = GetPreArchetype(archetype, removeCompId);
            //move comp data
            var chunkId = entityInternal.ArchetypeChunkId;
            foreach (var compId in archetype.compIds)
            {
                if (compId == removeCompId) continue;
                ref var compType = ref GetComponentTypeData(compId);
                compType.CopyChunkComponent(archetype, chunkId, preArchetype);
                //var comp = archetypeData.GetComponent<T>(compId, chunkId);
                //preArchetypeData.AddComponent<T>(compId, comp);
            }
            archetype.RemoveEntity(ref entityInternal);
            preArchetype.AddEntity(ref entityInternal);
            entityInternal.ComponentCount--;
            ComponentRemovedEvent?.Invoke(entityInternal, removeCompId);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetComponent<T>(EcsEntity entity, T comp) where T : struct
        {
            ref var entityInternal = ref entityInternals[entity.Index];
            if (entity.Version != entityInternal.Version) return;
            var compId = GetComponentId<T>();
            //SetComponent<T>(entityId, comp, compId);
            SetComponentInternal(ref entityInternal, comp, compId);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetComponent<T>(EcsEntity entity, T comp, byte compId) where T : struct
        {
            ref var entityInternal = ref entityInternals[entity.Index];
            if (entity.Version != entityInternal.Version) return;
            //var archetype = archetypeList[entityInternal.ArchetypeId];
            //archetype.SetComponent(compId, entityInternal.ArchetypeChunkId, comp);

            SetComponentInternal(ref entityInternal, comp, compId);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetComponentInternal<T>(ref EcsEntityInternal entity, T comp, byte compId) where T : struct
        {
            var archetype = archetypeList[entity.ArchetypeId];
            archetype.SetComponent(compId, entity.ArchetypeChunkId, comp);
        }
        public void SetComponent(EcsEntity entity, object comp)
        {
            if (comp == null) return;
            ref var entityInternal = ref entityInternals[entity.Index];
            if (entity.Version != entityInternal.Version) return;
            var compId = GetComponentId(comp.GetType());
            var archetype = archetypeList[entityInternal.ArchetypeId];
            archetype.SetComponent(compId, entityInternal.ArchetypeChunkId, comp);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public ref readonly EcsArchetype GetEntityArchetype(int entityId)
        public EcsArchetype GetEntityArchetype(EcsEntity entity)
        {
            ref var entityInternal = ref entityInternals[entity.Index];
            if (entity.Version != entityInternal.Version) return null;
            //ref var entity = ref entities[entityId];
            //ref var archetype = ref archetypes[entity.ArchetypeId];
            var archetype = archetypeList[entityInternal.ArchetypeId];
            return archetype;
        }
        public int GetComponents(EcsEntity entity, ref object[] comps)
        {
            ref var entityInternal = ref entityInternals[entity.Index];
            if (entity.Version != entityInternal.Version) return 0;
            //ref var entity = ref entities[entityId];
            //ref var archetype = ref archetypes[entity.ArchetypeId];
            var archetype = archetypeList[entityInternal.ArchetypeId];
            return archetype.GetComps(entityInternal.ArchetypeChunkId, ref comps);
        }
        public int GetComponentTypes(EcsEntity entity, ref Type[] types)
        {
            ref var entityInternal = ref entityInternals[entity.Index];
            if (entity.Version != entityInternal.Version) return 0;
            //ref var entity = ref entities[entityId];
            //ref var archetype = ref archetypes[entity.ArchetypeId];
            var archetype = archetypeList[entityInternal.ArchetypeId];
            var compCount = entityInternal.ComponentCount;
            if (compCount == 0) { return 0; }
            if (types == null || types.Length < compCount)
                types = new Type[compCount];
            for (int i = 0; i < compCount; i++)
            {
                types[i] = GetComponentType(archetype.compIds[i]);
            }
            return compCount;
        }
        #endregion

        #region Archetype
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public ref readonly EcsArchetype GetArchetype(int archetypeId) => ref archetypes[archetypeId];
        internal EcsArchetype GetArchetype(int archetypeId) => archetypeList[archetypeId];
        //internal ref EcsArchetype GetNextArchetype(ref EcsArchetype archetype, byte addCompId)
        internal EcsArchetype GetNextArchetype(EcsArchetype archetype, byte addCompId)
        {
            //var archetypeId = archetype.nextIds[addCompId];
            //if (archetypeId > 0)
            //    return archetypeList[archetypeId];
            var nextArchetype = archetype.nexts[addCompId];
            if (nextArchetype != null)
                return nextArchetype;
            //tempCompCount = archetypeData.compCount + 1;
            bool added = false;
            int length = 0;
            //for (int i = 0; i < archetypeData.compCount; i++)
            foreach (var compId in archetype.compIds)
            {
                //var compId = archetypeData.compIds[i];
                if (!added && addCompId < compId)
                {
                    tempCompIds[length++] = addCompId;
                    added = true;
                }
                tempCompIds[length++] = compId;
            }
            if (!added) tempCompIds[length++] = addCompId;
            //ref var nextArchetype = ref GetArchetype(tempCompIds, length);
            nextArchetype = GetArchetype(tempCompIds, length);
            //archetype.nextIds[addCompId] = nextArchetype.id;
            //nextArchetype.preIds[addCompId] = archetype.id;
            archetype.nexts[addCompId] = nextArchetype;
            nextArchetype.pres[addCompId] = archetype;
            //Console.WriteLine($"EcsWorld.GetNextArchetype: {archetype.id}->{nextArchetype.id}, {addCompId}");
            return nextArchetype;
        }
        //internal ref EcsArchetype GetPreArchetype(ref EcsArchetype archetype, byte removeCompId)
        internal EcsArchetype GetPreArchetype(EcsArchetype archetype, byte removeCompId)
        {
            //var archetypeId = archetype.preIds[removeCompId];
            //if (archetypeId > 0)
            //    return archetypeList[archetypeId];
            var preArchetype = archetype.pres[removeCompId];
            if (preArchetype != null)
                return preArchetype;
            //tempCompIds.Length = archetypeData.compCount - 1;
            int length = 0;
            foreach (var compId in archetype.compIds)
            {
                if (removeCompId != compId)
                    tempCompIds[length++] = compId;
            }
            preArchetype = GetArchetype(tempCompIds, length);
            //preArchetype.nextIds[removeCompId] = archetype.id;
            //archetype.preIds[removeCompId] = preArchetype.id;
            preArchetype.nexts[removeCompId] = archetype;
            archetype.pres[removeCompId] = preArchetype;
            //Console.WriteLine($"EcsWorld.GetPreArchetype: {archetype.id}->{preArchetype.id}, {removeCompId}");
            return preArchetype;
        }
        //public ref EcsArchetype GetArchetype(params Type[] types)
        //public EcsArchetype GetArchetype(params Type[] types)
        //{
        //    if (types == null || types.Length == 0)
        //        return archetypeList[0];
        //    var length = types.Length;
        //    //tempCompIds.Length = length;
        //    for (int i = 0; i < length; i++)
        //        tempCompIds[i] = GetComponentId(types[i]);
        //    //var compIds = GetCompnentIds(types);
        //    Array.Sort(tempCompIds, 0, length);
        //    return GetArchetype(tempCompIds, length);
        //}
        public EcsArchetype GetArchetype(params byte[] compIds)
        {
            if (compIds == null || compIds.Length == 0)
                return archetypeList[0];
            var length = compIds.Length;
            if (length > 1) Array.Sort(compIds);
            return GetArchetype(compIds, length);
        }
        //internal ref EcsArchetype GetArchetype(byte[] compIds, int count)
        internal EcsArchetype GetArchetype(byte[] compIds, int count)
        {
            var curArchetype = archetypeList[0];
            if (compIds == null || count == 0) return curArchetype;
            for (int i = 0; i < count; i++)
            {
                var compId = compIds[i];
                //var nextArchetypeId = curArchetype.nextIds[compId];
                //if (nextArchetypeId == 0)
                var nextArchetype = curArchetype.nexts[compId];
                if (nextArchetype == null)
                {
                    //create new
                    //if (archetypeCount == archetypes.Length) Array.Resize(ref archetypes, archetypeCount << 1);
                    //nextArchetypeId = archetypeCount++;
                    var nextArchetypeId = archetypeList.Count;
                    var archetype = new EcsArchetype(this, nextArchetypeId, compIds, i + 1);
                    //archetype.preIds[compId] = curArchetype.id;
                    //curArchetype.nextIds[compId] = nextArchetypeId;
                    archetype.pres[compId] = curArchetype;
                    curArchetype.nexts[compId] = archetype;
                    //archetypes[nextArchetypeId] = archetype;
                    archetypeList.Add(archetype);
                    //archetypeCount++;
                    //Console.WriteLine($"EcsArchetypeData.GetArchetypeData: {nextArchetypeId},[{string.Join(",", archetypeData.compIds)}]");
                    ArchetypeCreatedEvent?.Invoke(archetype);
                    nextArchetype = archetype;
                }
                //curArchetype = archetypeList[nextArchetypeId];
                curArchetype = nextArchetype;
            }
            return curArchetype;
        }
        #endregion

        #region Component
        //public byte RegisterComponent<T>() where T : struct
        //{
        //    var type = typeof(T);
        //    if (compTypeDict.TryGetValue(type, out var compType))
        //        return compType.Id;
        //    var compId = compTypeCount;
        //    compType = new EcsComponentType
        //    {
        //        Id = compId,
        //        Type = type,
        //        CompArray = new EcsComponentArray<T>(1),
        //        ComponentArrayCreator = (capacity) => new EcsComponentArray<T>(capacity),
        //        CopyChunkComponent = (src, chunkId, dest) =>
        //        {
        //            var comp = src.GetComponent<T>(compId, chunkId);
        //            dest.AddComponent(compId, comp);
        //        }
        //    };
        //    compTypeDict[type] = compType;
        //    compTypes[compId] = compType;
        //    compTypeCount++;
        //    return compId;
        //}
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref T GetDefaultComp<T>() where T : struct
        {
            return ref ((EcsComponentArray<T>)compTypeDict[typeof(T)].CompArray).Array[0];
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal byte[] GetCompnentIds(params Type[] types)
        //{
        //    for (int i = 0; i < types.Length; i++)
        //        tempCompIds[i] = GetComponentId(types[i]);
        //    return tempCompIds;
        //}
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte GetComponentId(Type type)
        {
            if (!compTypeDict.TryGetValue(type, out var compType)) throw new Exception($"{type} not registered!");
            return compType.Id;
        }
        public byte GetComponentId<T>() where T : struct
        {
            var type = typeof(T);
            if (compTypeDict.TryGetValue(type, out var compType))
                return compType.Id;
            var compId = compTypeCount;
            compType = new EcsComponentType
            {
                Id = compId,
                Type = type,
                CompArray = new EcsComponentArray<T>(1),
                ComponentArrayCreator = (capacity) => new EcsComponentArray<T>(capacity),
                CopyChunkComponent = (src, chunkId, dest) =>
                {
                    var comp = src.GetComponent<T>(compId, chunkId);
                    dest.AddComponent(compId, comp);
                }
            };
            compTypeDict[type] = compType;
            compTypes[compId] = compType;
            compTypeCount++;
            return compId;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Type GetComponentType(byte typeId)
        {
            return compTypes[typeId].Type;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref EcsComponentType GetComponentTypeData(byte typeId) => ref compTypes[typeId];

        #endregion

        #region Query

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EcsQueryDesc CreateQueryDesc() => recycledQueryDescCount > 0 ? recycledQueryDescs[--recycledQueryDescCount] : new EcsQueryDesc(this);
        //public EcsQuery GetQuery(int queryId) => queries[queryId];
        public EcsQuery GetQuery(EcsQueryDesc questDesc)
        {
            var hash = questDesc.hash;
            if (queryDict.TryGetValue(hash, out var query))
            {
                questDesc.Reset();
                if (recycledQueryDescCount == recycledQueryDescs.Length) Array.Resize(ref recycledQueryDescs, recycledQueryDescCount << 1);
                recycledQueryDescs[recycledQueryDescCount++] = questDesc;
                return query;
            }
            //if (queryCount == queries.Length)
            //{
            //    Array.Resize(ref queries, queryCount << 1);
            //    //Array.Resize(ref queryDescs, queryCount << 1);
            //}
            //queryId = queryCount++;
            query = new EcsQuery(this, questDesc);
            queryDict[hash] = query;
            queryList.Add(query);
            //queries[queryId] = query;
            //queryDescs[queryId] = questDesc;
            QueryCreatedEvent?.Invoke(query);
            return query;
        }
        //public ref readonly EcsQuery GetQuery(int queryId) => ref queries[queryId];
        //public ref readonly EcsQuery GetQuery(EcsQueryDesc questDesc)
        //{
        //    var hash = questDesc.hash;
        //    if (queryIdDict.TryGetValue(hash, out var queryId))
        //    {
        //        questDesc.Reset();
        //        if (recycledQueryDescCount == recycledQueryDescs.Length) Array.Resize(ref recycledQueryDescs, recycledQueryDescCount << 1);
        //        recycledQueryDescs[recycledQueryDescCount++] = questDesc;
        //        return ref queries[queryId];
        //    }
        //    if (queryCount == queries.Length)
        //    {
        //        Array.Resize(ref queries, queryCount << 1);
        //        Array.Resize(ref queryDescs, queryCount << 1);
        //    }
        //    queryId = queryCount++;
        //    var query = new EcsQuery(this, queryId, questDesc);
        //    queryIdDict[hash] = queryId;
        //    queries[queryId] = query;
        //    queryDescs[queryId] = questDesc;
        //    QueryCreatedEvent?.Invoke(queryId);
        //    return ref queries[queryId];
        //}
        #endregion

        #region System
        //public ISystem CreateSystem(Type type)
        //{

        //}
        public IEcsSystem GetSystem(Type type)
        {
            systemDict.TryGetValue(type, out var system);
            return system;
        }
        public void AddSystem(IEcsSystem system)
        {
            var type = system.GetType();
            if (systemDict.ContainsKey(type))
                return;
            systemDict.Add(type, system);
            systemList.Add(system);
            if (isInited)
            {
                system.Init(this);
                //if (system is IEcsInitSystem initSystem)
                //    initSystem.Init(this);
                //if (system is IEcsRunSystem runSystem)
                //    AddRunSystem(runSystem);
            }
        }
        public void DestroySystem(IEcsSystem system)
        {
            if (disposed) return;
            var type = system.GetType();
            if (systemDict.ContainsKey(type))
                return;
            systemDict.Remove(type);
            systemList.Remove(system);
            system.Dispose();
            //if (system is IEcsDestroySystem destroySystem)
            //    destroySystem.Destroy(this);
            //if (system is IEcsRunSystem runSystem)
            //    RemoveRunSystem(runSystem);
        }
        //private void AddRunSystem(IEcsRunSystem runSystem)
        //{
        //    if (runSystemCount == runSystems.Length) Array.Resize(ref runSystems, runSystemCount << 1);
        //    runSystems[runSystemCount++] = runSystem;
        //}
        //private void RemoveRunSystem(IEcsRunSystem runSystem)
        //{
        //    var index = Array.IndexOf(runSystems, runSystem);
        //    if (index < 0) return;
        //    runSystemCount--;
        //    for (var i = index; i < runSystemCount; i++)
        //        runSystems[i] = runSystems[i + 1];
        //}
        #endregion

        public struct Config
        {
            public int EntityCapacity;
            public int RecycledEntityCapacity;
            //public int ArchetypeCapacity;
            //public int QueryCapacity;
            //public int SystemCapacity;
            internal const int EntityCapacityDefault = 512;
            internal const int RecycledEntityCapacityDefault = 512;
            //internal const int ArchetypeCapacityDefault = 512;
            //internal const int QueryCapacityDefault = 512;
            //internal const int SystemCapacityDefault = 64;
        }
    }
    #endregion
}
