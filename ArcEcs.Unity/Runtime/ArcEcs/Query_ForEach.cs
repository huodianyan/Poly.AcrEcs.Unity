namespace Poly.ArcEcs
{
    public delegate void ForEachEC<T0>(Entity e, ref T0 t0) where T0 : struct;
    public delegate void ForEachECC<T0, T1>(Entity e, ref T0 t0, ref T1 t1) where T0 : struct where T1 : struct;
    public delegate void ForEachECCC<T0, T1, T2>(Entity e, ref T0 t0, ref T1 t1, ref T2 t2) where T0 : struct where T1 : struct where T2 : struct;
    public delegate void ForEachECCCC<T0, T1, T2, T3>(Entity e, ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3) where T0 : struct where T1 : struct where T2 : struct where T3 : struct;

    public delegate void ForEachC<T0>(ref T0 t0) where T0 : struct;
    public delegate void ForEachCC<T0, T1>(ref T0 t0, ref T1 t1) where T0 : struct where T1 : struct;
    public delegate void ForEachCCC<T0, T1, T2>(ref T0 t0, ref T1 t1, ref T2 t2) where T0 : struct where T1 : struct where T2 : struct;
    public delegate void ForEachCCCC<T0, T1, T2, T3>(ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3) where T0 : struct where T1 : struct where T2 : struct where T3 : struct;
    public partial class Query
    {
        public void ForEach<T0>(ForEachEC<T0> handler) where T0 : struct
        {
            var entities = world.entityInternals;
            var compId0 = world.GetComponentId<T0>();
            ForEach((in Archetype archetype) =>
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
            var compId0 = world.GetComponentId<T0>();
            var compId1 = world.GetComponentId<T1>();
            ForEach((in Archetype archetype) =>
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
            var compId0 = world.GetComponentId<T0>();
            var compId1 = world.GetComponentId<T1>();
            var compId2 = world.GetComponentId<T2>();
            ForEach((in Archetype archetype) =>
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
            var compId0 = world.GetComponentId<T0>();
            var compId1 = world.GetComponentId<T1>();
            var compId2 = world.GetComponentId<T2>();
            var compId3 = world.GetComponentId<T3>();
            ForEach((in Archetype archetype) =>
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
        public void ForEach<T0>(ForEachC<T0> handler) where T0 : struct
        {
            var compId0 = world.GetComponentId<T0>();
            ForEach((in Archetype archetype) =>
            {
                var entityCount = archetype.entityCount;
                if (entityCount == 0) return;
                var comps = archetype.GetComps<T0>(compId0);
                for (int i = entityCount - 1; i >= 0; i--)
                    handler(ref comps[i]);
            });
        }
        public void ForEach<T0, T1>(ForEachCC<T0, T1> handler) where T0 : struct where T1 : struct
        {
            var compId0 = world.GetComponentId<T0>();
            var compId1 = world.GetComponentId<T1>();
            ForEach((in Archetype archetype) =>
            {
                var entityCount = archetype.entityCount;
                if (entityCount == 0) return;
                var comp0s = archetype.GetComps<T0>(compId0);
                var comp1s = archetype.GetComps<T1>(compId1);
                for (int i = entityCount - 1; i >= 0; i--)
                    handler(ref comp0s[i], ref comp1s[i]);
            });
        }
        public void ForEach<T0, T1, T2>(ForEachCCC<T0, T1, T2> handler) where T0 : struct where T1 : struct where T2 : struct
        {
            var compId0 = world.GetComponentId<T0>();
            var compId1 = world.GetComponentId<T1>();
            var compId2 = world.GetComponentId<T2>();
            ForEach((in Archetype archetype) =>
            {
                var entityCount = archetype.entityCount;
                if (entityCount == 0) return;
                var comp0s = archetype.GetComps<T0>(compId0);
                var comp1s = archetype.GetComps<T1>(compId1);
                var comp2s = archetype.GetComps<T2>(compId2);
                for (int i = entityCount - 1; i >= 0; i--)
                    handler(ref comp0s[i], ref comp1s[i], ref comp2s[i]);
            });
        }
        public void ForEach<T0, T1, T2, T3>(ForEachCCCC<T0, T1, T2, T3> handler) where T0 : struct where T1 : struct where T2 : struct where T3 : struct
        {
            var compId0 = world.GetComponentId<T0>();
            var compId1 = world.GetComponentId<T1>();
            var compId2 = world.GetComponentId<T2>();
            var compId3 = world.GetComponentId<T3>();
            ForEach((in Archetype archetype) =>
            {
                var entityCount = archetype.entityCount;
                if (entityCount == 0) return;
                var comp0s = archetype.GetComps<T0>(compId0);
                var comp1s = archetype.GetComps<T1>(compId1);
                var comp2s = archetype.GetComps<T2>(compId2);
                var comp3s = archetype.GetComps<T3>(compId3);
                for (int i = entityCount - 1; i >= 0; i--)
                    handler(ref comp0s[i], ref comp1s[i], ref comp2s[i], ref comp3s[i]);
            });
        }
    }
}
