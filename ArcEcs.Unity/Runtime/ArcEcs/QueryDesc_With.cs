namespace Poly.ArcEcs
{
    public partial class QueryDesc
    {
        public QueryDesc WithAll<T0>() where T0 : struct => WithAll(compIds: world.GetComponentId<T0>());
        public QueryDesc WithAll<T0, T1>() where T0 : struct where T1 : struct => WithAll(world.GetComponentId<T0>(), world.GetComponentId<T1>());
        public QueryDesc WithAll<T0, T1, T2>() where T0 : struct where T1 : struct where T2 : struct => WithAll(world.GetComponentId<T0>(), world.GetComponentId<T1>(), world.GetComponentId<T2>());
        public QueryDesc WithAll<T0, T1, T2, T3>() where T0 : struct where T1 : struct where T2 : struct where T3 : struct => WithAll(world.GetComponentId<T0>(), world.GetComponentId<T1>(), world.GetComponentId<T2>(), world.GetComponentId<T3>());

        public QueryDesc WithAny<T0>() where T0 : struct => WithAny(compIds: world.GetComponentId<T0>());
        public QueryDesc WithAny<T0, T1>() where T0 : struct where T1 : struct => WithAny(world.GetComponentId<T0>(), world.GetComponentId<T1>());
        public QueryDesc WithAny<T0, T1, T2>() where T0 : struct where T1 : struct where T2 : struct => WithAny(world.GetComponentId<T0>(), world.GetComponentId<T1>(), world.GetComponentId<T2>());

        public QueryDesc WithNone<T0>() where T0 : struct => WithNone(compIds: world.GetComponentId<T0>());
        public QueryDesc WithNone<T0, T1>() where T0 : struct where T1 : struct => WithNone(world.GetComponentId<T0>(), world.GetComponentId<T1>());
        public QueryDesc WithNone<T0, T1, T2>() where T0 : struct where T1 : struct where T2 : struct => WithNone(world.GetComponentId<T0>(), world.GetComponentId<T1>(), world.GetComponentId<T2>());
    }
}
