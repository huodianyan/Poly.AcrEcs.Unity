using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poly.ArcEcs
{
    public static class EcsWorldExtensions
    {
        #region Entity
        public static EcsEntity CreateEntity<T0>(this EcsWorld world, T0 comp0 = default) where T0 : struct
        {
            var id0 = world.GetComponentId<T0>();
            ref var entity = ref world.CreateEntityInternal(id0);
            world.SetComponentInternal(ref entity, comp0, id0);
            return entity;
        }
        public static EcsEntity CreateEntity<T0, T1>(this EcsWorld world, T0 comp0 = default, T1 comp1 = default) where T0 : struct where T1 : struct
        {
            var id0 = world.GetComponentId<T0>();
            var id1 = world.GetComponentId<T1>();
            ref var entity = ref world.CreateEntityInternal(id0, id1);
            world.SetComponentInternal(ref entity, comp0, id0);
            world.SetComponentInternal(ref entity, comp1, id1);
            return entity;
        }
        public static EcsEntity CreateEntity<T0, T1, T2>(this EcsWorld world, T0 comp0 = default, T1 comp1 = default, T2 comp2 = default) where T0 : struct where T1 : struct where T2 : struct
        {
            var id0 = world.GetComponentId<T0>();
            var id1 = world.GetComponentId<T1>();
            var id2 = world.GetComponentId<T2>();
            ref var entity = ref world.CreateEntityInternal(id0, id1, id2);
            world.SetComponentInternal(ref entity, comp0, id0);
            world.SetComponentInternal(ref entity, comp1, id1);
            world.SetComponentInternal(ref entity, comp2, id2);
            return entity;
        }
        public static EcsEntity CreateEntity<T0, T1, T2, T3>(this EcsWorld world, T0 comp0 = default, T1 comp1 = default, T2 comp2 = default, T3 comp3 = default) where T0 : struct where T1 : struct where T2 : struct where T3 : struct
        {
            var id0 = world.GetComponentId<T0>();
            var id1 = world.GetComponentId<T1>();
            var id2 = world.GetComponentId<T2>();
            var id3 = world.GetComponentId<T3>();
            ref var entity = ref world.CreateEntityInternal(id0, id1, id2, id3);
            world.SetComponentInternal(ref entity, comp0, id0);
            world.SetComponentInternal(ref entity, comp1, id1);
            world.SetComponentInternal(ref entity, comp2, id2);
            world.SetComponentInternal(ref entity, comp3, id3);
            return entity;
        }
        #endregion

        #region Archetype
        public static EcsArchetype GetArchetype<T0>(this EcsWorld world) where T0 : struct
        {
            var id0 = world.GetComponentId<T0>();
            return world.GetArchetype(id0);
        }
        public static EcsArchetype GetArchetype<T0, T1>(this EcsWorld world) where T0 : struct where T1 : struct
        {
            var id0 = world.GetComponentId<T0>();
            var id1 = world.GetComponentId<T1>();
            return world.GetArchetype(id0, id1);
        }
        public static EcsArchetype GetArchetype<T0, T1, T2>(this EcsWorld world) where T0 : struct where T1 : struct where T2 : struct
        {
            var id0 = world.GetComponentId<T0>();
            var id1 = world.GetComponentId<T1>();
            var id2 = world.GetComponentId<T2>();
            return world.GetArchetype(id0, id1, id2);
        }
        public static EcsArchetype GetArchetype<T0, T1, T2, T3>(this EcsWorld world) where T0 : struct where T1 : struct where T2 : struct where T3 : struct
        {
            var id0 = world.GetComponentId<T0>();
            var id1 = world.GetComponentId<T1>();
            var id2 = world.GetComponentId<T2>();
            var id3 = world.GetComponentId<T3>();
            return world.GetArchetype(id0, id1, id2, id3);
        }
        #endregion
    }
}
