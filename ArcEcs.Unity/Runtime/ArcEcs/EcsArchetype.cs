using System;
using System.Runtime.CompilerServices;

namespace Poly.ArcEcs
{
    #region ArcheType
    //public struct EcsArchetype
    //{
    //    private readonly EcsArchetypeData data;
    //    public int Id => data.id;
    //    public int CompCount => data.compCount;
    //    public int EntityCount => data.entityIds.Count;

    //    internal EcsArchetype(EcsArchetypeData data)
    //    {
    //        this.data = data;
    //    }
    //}
    public class EcsArchetype
    {
        internal readonly EcsWorld world;
        internal readonly int id;
        internal readonly byte[] compIds;
        internal readonly byte compCount;
        internal readonly byte[] compIndexs;
        internal readonly byte compIndexCount;
        //private readonly FastArray<int> compIds;
        //internal readonly int hash;
        //internal FastList<int> entityIds;
        internal int[] entityIds;
        internal int entityCount;
        internal IEcsComponentArray[] compArrays;
        //internal FastArray<int> NextIds;
        //internal FastArray<int> PreIds;
        internal int[] NextIds;//index by comp
        internal int[] PreIds;

        public int Id => id;
        public int CompCount => compCount;
        public int EntityCount => entityCount;

        public event Action<int> EntityAddedEvent;
        public event Action<int> EntityRemovedEvent;

        internal EcsArchetype(EcsWorld world, int id, byte[] compIds, int count)//FastArray<int> compIds, int hash = 0
        {
            this.world = world;
            this.id = id;
            compCount = (byte)count;
            //entityIds = new FastList<int>(32);
            entityIds = new int[512];
            entityCount = 0;
            //NextIds = new FastArray<int>(32);
            //PreIds = new FastArray<int>(32);
            NextIds = new int[256];
            PreIds = new int[256];

            //this.compIds = null;
            this.compIds = new byte[compCount];
            compIndexCount = 0;
            compIndexs = null;
            compArrays = null;
            if (count > 0)
            {
                Array.Copy(compIds, this.compIds, compCount);
                //this.hash = hash != 0 ? hash : GetHash(compIds);
                compIndexCount = (byte)(this.compIds[compCount - 1] + 1);
                compIndexs = new byte[compIndexCount];
                for (int i = 0; i < compIndexCount; i++) compIndexs[i] = 0xff;
                compArrays = new IEcsComponentArray[compCount];
                for (byte i = 0; i < compCount; i++)
                {
                    var compId = this.compIds[i];
                    var compTypeData = world.GetComponentTypeData(compId);
                    compArrays[i] = compTypeData.ComponentArrayCreator.Invoke(512);
                    compIndexs[compId] = i;
                }
            }
        }
        internal void Dispose()
        {
            entityIds = null;
            compArrays = null;
            NextIds = null;
            PreIds = null;
        }
        public int GetComps(int chunkId, ref object[] comps)
        {
            if (compCount == 0) { return 0; }
            if (comps == null || comps.Length < compCount)
                comps = new object[compCount];
            for (int i = 0; i < compCount; i++)
                comps[i] = compArrays[i].Get(chunkId);
            return compCount;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] GetComps<T>(byte compId) where T : struct
        {
            var list = (EcsComponentArray<T>)compArrays[compIndexs[compId]];
            return list.Array;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool HasComponent(int compId)
        {
            return compId < compIndexCount && compIndexs[compId] != 0xff;
        }

        internal int AddEntity(ref EcsEntity entity)
        {
            var chunkId = entityCount;
            //entityIds.Add(entityData.Index);
            if (entityCount == entityIds.Length) Array.Resize(ref entityIds, entityCount << 1);
            entityIds[entityCount++] = entity.Index;
            //Console.WriteLine($"EcsArchetypeData.AddEntity: {entityList.Count}");
            entity.ArchetypeId = id;
            entity.ArchetypeChunkId = chunkId;
            for (int i = 0; i < compCount; i++)
                compArrays[i].Add();
            EntityAddedEvent?.Invoke(entity.Index);
            return chunkId;
        }
        internal void RemoveEntity(ref EcsEntity entity)
        {
            var chunkId = entity.ArchetypeChunkId;
            entity.ArchetypeId = -1;
            entity.ArchetypeChunkId = -1;
            //Console.WriteLine($"EcsArchetypeData.RemoveEntity: entityCount[{entityIds.Count}],compCount[{compCount}],chunkId[{chunkId}]");
            if (--entityCount > chunkId)
            {
                entityIds[chunkId] = entityIds[entityCount];
                var swapEntityId = entityIds[chunkId];
                ref var swapEntity = ref world.entities[swapEntityId];
                swapEntity.ArchetypeChunkId = chunkId;
            }
            //Console.WriteLine($"FastList.RemoveAtSwap: {count}, {index}");
            entityIds[entityCount] = default;
            //if (entityIds.RemoveAtSwap(chunkId))
            //{
            //    var swapEntityId = entityIds[chunkId];
            //    entityData = ref world.GetEntityData(swapEntityId);
            //    entityData.ArchetypeChunkId = chunkId;
            //}
            for (int i = 0; i < compCount; i++)
                compArrays[i].RemoveAt(chunkId);
            EntityRemovedEvent?.Invoke(entity.Index);
        }
        internal ref T AddComponent<T>(int compId) where T : struct
        {
            var compIndex = compIndexs[compId];
            var list = (EcsComponentArray<T>)compArrays[compIndex];
            var chunkId = list.Add(default);
            return ref list.Array[chunkId];
        }
        internal void AddComponent<T>(int compId, T comp) where T : struct
        {
            if (!HasComponent(compId))
                return;
            var compIndex = compIndexs[compId];
            var list = (EcsComponentArray<T>)compArrays[compIndex];
            list.Add(comp);
        }
        internal void SetComponent<T>(int compId, int chunkId, T comp) where T : struct
        {
            if (!HasComponent(compId))
                return;
            var compIndex = compIndexs[compId];
            var list = (EcsComponentArray<T>)compArrays[compIndex];
            list.Array[chunkId] = comp;
        }
        internal void SetComponent(int compId, int chunkId, object comp)
        {
            if (!HasComponent(compId))
                return;
            compArrays[compIndexs[compId]].Set(chunkId, comp);
        }
        //internal object GetComponent(int compId, int chunkId)
        //{
        //    if (!HasComponent(compId))
        //        return default;
        //    var compIndex = compIndexs[compId];
        //    return compLists[compIndex][chunkId];
        //}
        internal ref T GetComponent<T>(int compId, int chunkId) where T : struct
        {
            //if (!HasComponent(compId))
            //    return default;
            var compIndex = compIndexs[compId];
            var list = (EcsComponentArray<T>)compArrays[compIndex];
            return ref list.Array[chunkId];
        }
        //public static int GetHash(FastArray<int> compIds)
        //{
        //    var compCount = compIds.Length;
        //    Array.Sort(compIds.Array, 0, compCount);
        //    var hash = compCount;
        //    for (int i = 0; i < compCount; i++)
        //        hash = unchecked(hash * 17 + compIds[i]);
        //    return hash;
        //}
    }
    #endregion
}
