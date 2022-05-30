namespace Poly.ArcEcs
{
    public partial class World
    {
        #region Entity
        public Entity CreateEntity<T0>(T0 comp0 = default) where T0 : struct
        {
            var id0 = GetComponentId<T0>();
            ref var entity = ref CreateEntityInternal(id0);
            SetComponentInternal(ref entity, comp0, id0);
            return entity;
        }
        public Entity CreateEntity<T0, T1>(T0 comp0 = default, T1 comp1 = default) where T0 : struct where T1 : struct
        {
            var id0 = GetComponentId<T0>();
            var id1 = GetComponentId<T1>();
            ref var entity = ref CreateEntityInternal(id0, id1);
            SetComponentInternal(ref entity, comp0, id0);
            SetComponentInternal(ref entity, comp1, id1);
            return entity;
        }
        public Entity CreateEntity<T0, T1, T2>(T0 comp0 = default, T1 comp1 = default, T2 comp2 = default) where T0 : struct where T1 : struct where T2 : struct
        {
            var id0 = GetComponentId<T0>();
            var id1 = GetComponentId<T1>();
            var id2 = GetComponentId<T2>();
            ref var entity = ref CreateEntityInternal(id0, id1, id2);
            SetComponentInternal(ref entity, comp0, id0);
            SetComponentInternal(ref entity, comp1, id1);
            SetComponentInternal(ref entity, comp2, id2);
            return entity;
        }
        public Entity CreateEntity<T0, T1, T2, T3>(T0 comp0 = default, T1 comp1 = default, T2 comp2 = default, T3 comp3 = default) where T0 : struct where T1 : struct where T2 : struct where T3 : struct
        {
            var id0 = GetComponentId<T0>();
            var id1 = GetComponentId<T1>();
            var id2 = GetComponentId<T2>();
            var id3 = GetComponentId<T3>();
            ref var entity = ref CreateEntityInternal(id0, id1, id2, id3);
            SetComponentInternal(ref entity, comp0, id0);
            SetComponentInternal(ref entity, comp1, id1);
            SetComponentInternal(ref entity, comp2, id2);
            SetComponentInternal(ref entity, comp3, id3);
            return entity;
        }
        #endregion

        #region Archetype
        public Archetype GetArchetype<T0>() where T0 : struct
        {
            var id0 = GetComponentId<T0>();
            return GetArchetype(id0);
        }
        public Archetype GetArchetype<T0, T1>() where T0 : struct where T1 : struct
        {
            var id0 = GetComponentId<T0>();
            var id1 = GetComponentId<T1>();
            return GetArchetype(id0, id1);
        }
        public Archetype GetArchetype<T0, T1, T2>() where T0 : struct where T1 : struct where T2 : struct
        {
            var id0 = GetComponentId<T0>();
            var id1 = GetComponentId<T1>();
            var id2 = GetComponentId<T2>();
            return GetArchetype(id0, id1, id2);
        }
        public Archetype GetArchetype<T0, T1, T2, T3>() where T0 : struct where T1 : struct where T2 : struct where T3 : struct
        {
            var id0 = GetComponentId<T0>();
            var id1 = GetComponentId<T1>();
            var id2 = GetComponentId<T2>();
            var id3 = GetComponentId<T3>();
            return GetArchetype(id0, id1, id2, id3);
        }
        #endregion
        public T GetSystem<T>() where T : ISystem
        {
            return (T)GetSystem(typeof(T));
        }
    }
    //public static class WorldExtensions
    //{
    //    #region Entity
    //    public static Entity CreateEntity<T0>(this World world, T0 comp0 = default) where T0 : struct
    //    {
    //        var id0 = world.GetComponentId<T0>();
    //        ref var entity = ref world.CreateEntityInternal(id0);
    //        world.SetComponentInternal(ref entity, comp0, id0);
    //        return entity;
    //    }
    //    public static Entity CreateEntity<T0, T1>(this World world, T0 comp0 = default, T1 comp1 = default) where T0 : struct where T1 : struct
    //    {
    //        var id0 = world.GetComponentId<T0>();
    //        var id1 = world.GetComponentId<T1>();
    //        ref var entity = ref world.CreateEntityInternal(id0, id1);
    //        world.SetComponentInternal(ref entity, comp0, id0);
    //        world.SetComponentInternal(ref entity, comp1, id1);
    //        return entity;
    //    }
    //    public static Entity CreateEntity<T0, T1, T2>(this World world, T0 comp0 = default, T1 comp1 = default, T2 comp2 = default) where T0 : struct where T1 : struct where T2 : struct
    //    {
    //        var id0 = world.GetComponentId<T0>();
    //        var id1 = world.GetComponentId<T1>();
    //        var id2 = world.GetComponentId<T2>();
    //        ref var entity = ref world.CreateEntityInternal(id0, id1, id2);
    //        world.SetComponentInternal(ref entity, comp0, id0);
    //        world.SetComponentInternal(ref entity, comp1, id1);
    //        world.SetComponentInternal(ref entity, comp2, id2);
    //        return entity;
    //    }
    //    public static Entity CreateEntity<T0, T1, T2, T3>(this World world, T0 comp0 = default, T1 comp1 = default, T2 comp2 = default, T3 comp3 = default) where T0 : struct where T1 : struct where T2 : struct where T3 : struct
    //    {
    //        var id0 = world.GetComponentId<T0>();
    //        var id1 = world.GetComponentId<T1>();
    //        var id2 = world.GetComponentId<T2>();
    //        var id3 = world.GetComponentId<T3>();
    //        ref var entity = ref world.CreateEntityInternal(id0, id1, id2, id3);
    //        world.SetComponentInternal(ref entity, comp0, id0);
    //        world.SetComponentInternal(ref entity, comp1, id1);
    //        world.SetComponentInternal(ref entity, comp2, id2);
    //        world.SetComponentInternal(ref entity, comp3, id3);
    //        return entity;
    //    }
    //    #endregion

    //    #region Archetype
    //    public static Archetype GetArchetype<T0>(this World world) where T0 : struct
    //    {
    //        var id0 = world.GetComponentId<T0>();
    //        return world.GetArchetype(id0);
    //    }
    //    public static Archetype GetArchetype<T0, T1>(this World world) where T0 : struct where T1 : struct
    //    {
    //        var id0 = world.GetComponentId<T0>();
    //        var id1 = world.GetComponentId<T1>();
    //        return world.GetArchetype(id0, id1);
    //    }
    //    public static Archetype GetArchetype<T0, T1, T2>(this World world) where T0 : struct where T1 : struct where T2 : struct
    //    {
    //        var id0 = world.GetComponentId<T0>();
    //        var id1 = world.GetComponentId<T1>();
    //        var id2 = world.GetComponentId<T2>();
    //        return world.GetArchetype(id0, id1, id2);
    //    }
    //    public static Archetype GetArchetype<T0, T1, T2, T3>(this World world) where T0 : struct where T1 : struct where T2 : struct where T3 : struct
    //    {
    //        var id0 = world.GetComponentId<T0>();
    //        var id1 = world.GetComponentId<T1>();
    //        var id2 = world.GetComponentId<T2>();
    //        var id3 = world.GetComponentId<T3>();
    //        return world.GetArchetype(id0, id1, id2, id3);
    //    }
    //    #endregion
    //}
}
