using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Poly.ArcEcs
{
    internal delegate void ForEachArchetype(in EcsArchetype archetypeData);
    public delegate void ForEachE(EcsEntity e);
    public partial class EcsQuery
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
            var compId0 = world.GetComponentId<T>();
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
        #endregion
    }
}
