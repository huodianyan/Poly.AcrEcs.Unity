using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Poly.ArcEcs
{
    #region Query
    internal delegate void ForEachArchetype(in EcsArchetype archetypeData);
    public delegate void ForEachE(EcsEntity e);
    public delegate void ForEachEC<T0>(EcsEntity e, ref T0 t0) where T0 : struct;
    public delegate void ForEachECC<T0, T1>(EcsEntity e, ref T0 t0, ref T1 t1) where T0 : struct where T1 : struct;
    public delegate void ForEachECCC<T0, T1, T2>(EcsEntity e, ref T0 t0, ref T1 t1, ref T2 t2) where T0 : struct where T1 : struct where T2 : struct;
    public delegate void ForEachECCCC<T0, T1, T2, T3>(EcsEntity e, ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3) where T0 : struct where T1 : struct where T2 : struct where T3 : struct;

    public class EcsQuery
    {
        private readonly EcsWorld world;
        private readonly EcsQueryDesc queryDesc;
        //internal int id;
        internal int version;
        //internal FastArray<int> ArchetypeIds;
        //internal int[] archetypeIds;
        //internal int archetypeCount;
        internal List<EcsArchetype> archetypeList;

        //public int Version => version;
        //public ref readonly EcsQueryDesc QueryDesc => ref world.queryDescs[id];
        public EcsQueryDesc QueryDesc => queryDesc;
        public int Hash => queryDesc.hash;

        public event Action<EcsArchetype> ArchetypeAddedEvent;

        internal EcsQuery(EcsWorld world, EcsQueryDesc queryDesc)
        {
            this.world = world;
            //this.id = id;
            this.queryDesc = queryDesc;
            version = 0;
            //archetypeIds = new int[256];
            //archetypeCount = 0;
            archetypeList = new List<EcsArchetype>();
            //ArchetypeAddedEvent = null;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetEntityCount()
        {
            UpdateArchetypes();
            int count = 0;
            for (int i = 0; i < archetypeList.Count; i++)
            {
                //ref readonly var archetypeData = ref world.archetypes[archetypeIds[i]];
                var archetype = archetypeList[i];
                count += archetype.entityCount;
            }
            return count;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Matchs(EcsEntity entity)
        {
            //ref readonly var archetype = ref world.GetEntityArchetype(entityId);
            var archetype = world.GetEntityArchetype(entity);
            return Marches(archetype);
        }
        internal bool Marches(in EcsArchetype archetype)
        {
            for (int i = 0; i < queryDesc.allCount; i++)
                if (!archetype.HasComponent(queryDesc.all[i])) return false;
            for (int i = 0; i < queryDesc.noneCount; i++)
                if (archetype.HasComponent(queryDesc.none[i])) return false;
            if (queryDesc.anyCount == 0) return true;
            bool isAny = false;
            for (int i = 0; i < queryDesc.anyCount; i++)
                if (archetype.HasComponent(queryDesc.any[i])) { isAny = true; break; }
            return isAny;
        }
        internal void UpdateArchetypes()
        {
            var worldArchetypeList = world.archetypeList;
            //var count = world.archetypeCount;
            var count = worldArchetypeList.Count;
            //Console.WriteLine($"EcsQuery.UpdateArchetypes: {version} == {count}, {id}");
            if (version >= count) return;
            //ref readonly var queryDesc = ref world.queryDescs[id];
            for (int i = version; i < count; i++)
            {
                //ref var archetype = ref world.archetypes[i];
                var archetype = worldArchetypeList[i];
                //Console.WriteLine($"EcsQuery.UpdateArchetypes: {archetype.id}");
                if (!Marches(in archetype)) continue;
                //if (archetypeCount == archetypeIds.Length) Array.Resize(ref archetypeIds, archetypeCount << 1);
                //archetypeIds[archetypeCount++] = i;
                archetypeList.Add(archetype);
                ArchetypeAddedEvent?.Invoke(archetype);
            }
            version = count;
        }

        public int[] ToEntityArray(int[] entityIds, int count = 0)
        {
            UpdateArchetypes();
            int index = 0;
            if (count == 0) count = GetEntityCount();
            if (entityIds == null) entityIds = new int[count];
            else if (entityIds.Length < count) Array.Resize(ref entityIds, count);
            if (entityIds == null || entityIds.Length < count)
            {
                var count2 = EcsUtil.NextPowerOf2(count);
                entityIds = new int[count2];
            }
            ForEach((in EcsArchetype archetype) =>
            {
                var entityCount = archetype.entityCount;
                var ids = archetype.entityIds;
                for (int i = 0; i < entityCount; i++)
                    entityIds[index++] = ids[i];
            });
            return entityIds;
        }
        public T[] ToComponentArray<T>(T[] comps, int count = 0) where T : struct
        {
            UpdateArchetypes();
            int index = 0;
            if (count == 0) count = GetEntityCount();
            if (comps == null || comps.Length < count)
            {
                var count2 = EcsUtil.NextPowerOf2(count);
                comps = new T[count2];
            }
            var compId0 = world.GetComponentId(typeof(T));
            ForEach((in EcsArchetype archetype) =>
            {
                var entityCount = archetype.entityCount;
                var archetypeComps = archetype.GetComps<T>(compId0);
                for (int i = 0; i < entityCount; i++)
                    comps[index++] = archetypeComps[i];
            });
            return comps;
        }
        #region ForEach
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ForEach(ForEachArchetype handler)
        {
            UpdateArchetypes();
            var archetypeCount = archetypeList.Count;
            for (int i = 0; i < archetypeCount; i++)
            {
                //ref var archetypeData = ref world.archetypes[archetypeIds[i]];
                var archetype = archetypeList[i];
                if (archetype.entityCount > 0) handler(in archetype);
            }
        }
        public void ForEach(ForEachE handler)
        {
            var entities = world.entityInternals;
            ForEach((in EcsArchetype archetype) =>
            {
                var entityCount = archetype.entityCount;
                if (entityCount == 0) return;
                var entityIds = archetype.entityIds;
                //for (int i = 0; i < entityCount; i++)
                for (int i = entityCount - 1; i >= 0; i--)
                    handler(entities[entityIds[i]]);
            });
        }
        public void ForEach<T0>(ForEachEC<T0> handler) where T0 : struct
        {
            var entities = world.entityInternals;
            var compId0 = world.GetComponentId(typeof(T0));
            ForEach((in EcsArchetype archetype) =>
            {
                var entityCount = archetype.entityCount;
                if (entityCount == 0) return;
                var entityIds = archetype.entityIds;
                var comps = archetype.GetComps<T0>(compId0);
                //for (int i = 0; i < entityCount; i++)
                for (int i = entityCount - 1; i >= 0; i--)
                    handler(entities[entityIds[i]], ref comps[i]);
            });
        }
        public void ForEach<T0, T1>(ForEachECC<T0, T1> handler) where T0 : struct where T1 : struct
        {
            var entities = world.entityInternals;
            var compId0 = world.GetComponentId(typeof(T0));
            var compId1 = world.GetComponentId(typeof(T1));
            ForEach((in EcsArchetype archetype) =>
            {
                var entityCount = archetype.entityCount;
                if (entityCount == 0) return;
                var entityIds = archetype.entityIds;
                var comp0s = archetype.GetComps<T0>(compId0);
                var comp1s = archetype.GetComps<T1>(compId1);
                //for (int i = 0; i < entityCount; i++)
                for (int i = entityCount - 1; i >= 0; i--)
                    handler(entities[entityIds[i]], ref comp0s[i], ref comp1s[i]);
            });
        }
        public void ForEach<T0, T1, T2>(ForEachECCC<T0, T1, T2> handler) where T0 : struct where T1 : struct where T2 : struct
        {
            var entities = world.entityInternals;
            var compId0 = world.GetComponentId(typeof(T0));
            var compId1 = world.GetComponentId(typeof(T1));
            var compId2 = world.GetComponentId(typeof(T2));
            ForEach((in EcsArchetype archetype) =>
            {
                var entityCount = archetype.entityCount;
                if (entityCount == 0) return;
                var entityIds = archetype.entityIds;
                var comp0s = archetype.GetComps<T0>(compId0);
                var comp1s = archetype.GetComps<T1>(compId1);
                var comp2s = archetype.GetComps<T2>(compId2);
                //for (int i = 0; i < entityCount; i++)
                for (int i = entityCount - 1; i >= 0; i--)
                    handler(entities[entityIds[i]], ref comp0s[i], ref comp1s[i], ref comp2s[i]);
            });
        }
        public void ForEach<T0, T1, T2, T3>(ForEachECCCC<T0, T1, T2, T3> handler) where T0 : struct where T1 : struct where T2 : struct where T3 : struct
        {
            var entities = world.entityInternals;
            var compId0 = world.GetComponentId(typeof(T0));
            var compId1 = world.GetComponentId(typeof(T1));
            var compId2 = world.GetComponentId(typeof(T2));
            var compId3 = world.GetComponentId(typeof(T3));
            ForEach((in EcsArchetype archetype) =>
            {
                var entityCount = archetype.entityCount;
                if (entityCount == 0) return;
                var entityIds = archetype.entityIds;
                var comp0s = archetype.GetComps<T0>(compId0);
                var comp1s = archetype.GetComps<T1>(compId1);
                var comp2s = archetype.GetComps<T2>(compId2);
                var comp3s = archetype.GetComps<T3>(compId3);
                //for (int i = 0; i < entityCount; i++)
                for (int i = entityCount - 1; i >= 0; i--)
                    handler(entities[entityIds[i]], ref comp0s[i], ref comp1s[i], ref comp2s[i], ref comp3s[i]);
            });
        }
        #endregion
    }

    public class EcsQueryDesc
    {
        private readonly EcsWorld world;

        internal byte[] all;
        internal byte[] any;
        internal byte[] none;
        internal byte allCount;
        internal byte anyCount;
        internal byte noneCount;
        internal int hash;
        public ArraySegment<byte> All => new ArraySegment<byte>(all, 0, allCount);
        public ArraySegment<byte> Any => new ArraySegment<byte>(any, 0, anyCount);
        public ArraySegment<byte> None => new ArraySegment<byte>(none, 0, noneCount);

        internal EcsQueryDesc(EcsWorld world)
        {
            this.world = world;
            all = new byte[4];
            any = new byte[4];
            none = new byte[4];
            hash = allCount = anyCount = noneCount = 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            allCount = 0;
            anyCount = 0;
            noneCount = 0;
            hash = 0;
        }
        public EcsQueryDesc Build()
        {
            if (allCount > 1) Array.Sort(all, 0, allCount);
            if (anyCount > 1) Array.Sort(any, 0, anyCount);
            if (noneCount > 1) Array.Sort(none, 0, noneCount);
            // calculate hash.
            hash = allCount + anyCount + noneCount;
            for (int i = 0; i < allCount; i++)
                hash = unchecked(hash * 13 + all[i]);
            for (int i = 0; i < anyCount; i++)
                hash = unchecked(hash * 17 + any[i]);
            for (int i = 0; i < noneCount; i++)
                hash = unchecked(hash * 23 - none[i]);
            return this;
        }
        //internal bool Marches(in EcsArchetype archetype)
        //{
        //    for (int i = 0; i < allCount; i++)
        //        if (!archetype.HasComponent(all[i])) return false;
        //    for (int i = 0; i < noneCount; i++)
        //        if (archetype.HasComponent(none[i])) return false;
        //    if (anyCount == 0) return true;
        //    bool isAny = false;
        //    for (int i = 0; i < anyCount; i++)
        //        if (archetype.HasComponent(any[i])) { isAny = true; break; }
        //    return isAny;
        //}

        #region All
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WithAll(byte compId)
        {
            //if (Array.IndexOf(All, compId, 0, AllCount) != -1) { throw new Exception($"{type.Name} already in constraints list."); }
            //if (Array.IndexOf(None, compId, 0, NoneCount) != -1) { throw new Exception($"{type.Name} already in constraints list."); }
            if (allCount == all.Length) { Array.Resize(ref all, allCount << 1); }
            all[allCount++] = compId;
        }
        public EcsQueryDesc WithAll(params byte[] compIds)
        {
            foreach (var compId in compIds) WithAll(compId);
            return this;
        }
        public EcsQueryDesc WithAll(params Type[] types)
        {
            foreach (var type in types) WithAll(world.GetComponentId(type));
            return this;
        }
        public EcsQueryDesc WithAll<T0>() where T0 : struct => WithAll(typeof(T0));
        public EcsQueryDesc WithAll<T0, T1>() where T0 : struct where T1 : struct => WithAll(typeof(T0), typeof(T1));
        public EcsQueryDesc WithAll<T0, T1, T2>() where T0 : struct where T1 : struct where T2 : struct => WithAll(typeof(T0), typeof(T1), typeof(T2));
        public EcsQueryDesc WithAll<T0, T1, T2, T3>() where T0 : struct where T1 : struct where T2 : struct where T3 : struct => WithAll(typeof(T0), typeof(T1), typeof(T2), typeof(T3));
        #endregion

        #region Any
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WithAny(byte compId)
        {
            if (anyCount == any.Length) { Array.Resize(ref any, anyCount << 1); }
            any[anyCount++] = compId;
        }
        public EcsQueryDesc WithAny(params byte[] compIds)
        {
            foreach (var compId in compIds) WithAny(compId);
            return this;
        }
        public EcsQueryDesc WithAny(params Type[] types)
        {
            foreach (var type in types) WithAny(world.GetComponentId(type));
            return this;
        }
        public EcsQueryDesc WithAny<T0>() where T0 : struct => WithAny(typeof(T0));
        public EcsQueryDesc WithAny<T0, T1>() where T0 : struct where T1 : struct => WithAny(typeof(T0), typeof(T1));
        public EcsQueryDesc WithAny<T0, T1, T2>() where T0 : struct where T1 : struct where T2 : struct => WithAny(typeof(T0), typeof(T1), typeof(T2));
        #endregion

        #region None
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WithNone(byte compId)
        {
            if (noneCount == none.Length) { Array.Resize(ref none, noneCount << 1); }
            none[noneCount++] = compId;
        }
        public EcsQueryDesc WithNone(params byte[] compIds)
        {
            foreach (var compId in compIds) WithNone(compId);
            return this;
        }
        public EcsQueryDesc WithNone(params Type[] types)
        {
            foreach (var type in types) WithNone(world.GetComponentId(type));
            return this;
        }
        public EcsQueryDesc WithNone<T0>() where T0 : struct => WithNone(typeof(T0));
        public EcsQueryDesc WithNone<T0, T1>() where T0 : struct where T1 : struct => WithNone(typeof(T0), typeof(T1));
        public EcsQueryDesc WithNone<T0, T1, T2>() where T0 : struct where T1 : struct where T2 : struct => WithNone(typeof(T0), typeof(T1), typeof(T2));
        #endregion
    }
    #endregion
}
