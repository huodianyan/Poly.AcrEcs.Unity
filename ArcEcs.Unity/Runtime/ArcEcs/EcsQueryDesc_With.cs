namespace Poly.ArcEcs
{
    public partial class EcsQueryDesc
    {
        public EcsQueryDesc WithAll<T0>() where T0 : struct => WithAll(compIds: world.GetComponentId<T0>());
        public EcsQueryDesc WithAll<T0, T1>() where T0 : struct where T1 : struct => WithAll(world.GetComponentId<T0>(), world.GetComponentId<T1>());
        public EcsQueryDesc WithAll<T0, T1, T2>() where T0 : struct where T1 : struct where T2 : struct => WithAll(world.GetComponentId<T0>(), world.GetComponentId<T1>(), world.GetComponentId<T2>());
        public EcsQueryDesc WithAll<T0, T1, T2, T3>() where T0 : struct where T1 : struct where T2 : struct where T3 : struct => WithAll(world.GetComponentId<T0>(), world.GetComponentId<T1>(), world.GetComponentId<T2>(), world.GetComponentId<T3>());

        public EcsQueryDesc WithAny<T0>() where T0 : struct => WithAny(compIds: world.GetComponentId<T0>());
        public EcsQueryDesc WithAny<T0, T1>() where T0 : struct where T1 : struct => WithAny(world.GetComponentId<T0>(), world.GetComponentId<T1>());
        public EcsQueryDesc WithAny<T0, T1, T2>() where T0 : struct where T1 : struct where T2 : struct => WithAny(world.GetComponentId<T0>(), world.GetComponentId<T1>(), world.GetComponentId<T2>());

        public EcsQueryDesc WithNone<T0>() where T0 : struct => WithNone(compIds: world.GetComponentId<T0>());
        public EcsQueryDesc WithNone<T0, T1>() where T0 : struct where T1 : struct => WithNone(world.GetComponentId<T0>(), world.GetComponentId<T1>());
        public EcsQueryDesc WithNone<T0, T1, T2>() where T0 : struct where T1 : struct where T2 : struct => WithNone(world.GetComponentId<T0>(), world.GetComponentId<T1>(), world.GetComponentId<T2>());
    }
}
