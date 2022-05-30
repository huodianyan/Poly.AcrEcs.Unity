using System;
using System.Runtime.CompilerServices;

namespace Poly.ArcEcs
{
    public partial class QueryDesc
    {
        protected readonly World world;

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

        internal QueryDesc(World world)
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
        public QueryDesc Build()
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

        #region All
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WithAll(byte compId)
        {
            //if (Array.IndexOf(All, compId, 0, AllCount) != -1) { throw new Exception($"{type.Name} already in constraints list."); }
            //if (Array.IndexOf(None, compId, 0, NoneCount) != -1) { throw new Exception($"{type.Name} already in constraints list."); }
            if (allCount == all.Length) { Array.Resize(ref all, allCount << 1); }
            all[allCount++] = compId;
        }
        public QueryDesc WithAll(params byte[] compIds)
        {
            foreach (var compId in compIds) WithAll(compId);
            return this;
        }
        //public EcsQueryDesc WithAll(params Type[] types)
        //{
        //    foreach (var type in types) WithAll(world.GetComponentId(type));
        //    return this;
        //}
        //public EcsQueryDesc WithAll<T0>() where T0 : struct => WithAll(compIds: world.GetComponentId<T0>());
        //public EcsQueryDesc WithAll<T0, T1>() where T0 : struct where T1 : struct => WithAll(world.GetComponentId<T0>(), world.GetComponentId<T1>());
        //public EcsQueryDesc WithAll<T0, T1, T2>() where T0 : struct where T1 : struct where T2 : struct => WithAll(world.GetComponentId<T0>(), world.GetComponentId<T1>(), world.GetComponentId<T2>());
        //public EcsQueryDesc WithAll<T0, T1, T2, T3>() where T0 : struct where T1 : struct where T2 : struct where T3 : struct => WithAll(world.GetComponentId<T0>(), world.GetComponentId<T1>(), world.GetComponentId<T2>(), world.GetComponentId<T3>());
        #endregion

        #region Any
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WithAny(byte compId)
        {
            if (anyCount == any.Length) { Array.Resize(ref any, anyCount << 1); }
            any[anyCount++] = compId;
        }
        public QueryDesc WithAny(params byte[] compIds)
        {
            foreach (var compId in compIds) WithAny(compId);
            return this;
        }
        //public EcsQueryDesc WithAny(params Type[] types)
        //{
        //    foreach (var type in types) WithAny(world.GetComponentId(type));
        //    return this;
        //}
        //public EcsQueryDesc WithAny<T0>() where T0 : struct => WithAny(compIds: world.GetComponentId<T0>());
        //public EcsQueryDesc WithAny<T0, T1>() where T0 : struct where T1 : struct => WithAny(world.GetComponentId<T0>(), world.GetComponentId<T1>());
        //public EcsQueryDesc WithAny<T0, T1, T2>() where T0 : struct where T1 : struct where T2 : struct => WithAny(world.GetComponentId<T0>(), world.GetComponentId<T1>(), world.GetComponentId<T2>());
        #endregion

        #region None
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WithNone(byte compId)
        {
            if (noneCount == none.Length) { Array.Resize(ref none, noneCount << 1); }
            none[noneCount++] = compId;
        }
        public QueryDesc WithNone(params byte[] compIds)
        {
            foreach (var compId in compIds) WithNone(compId);
            return this;
        }
        //public EcsQueryDesc WithNone(params Type[] types)
        //{
        //    foreach (var type in types) WithNone(world.GetComponentId(type));
        //    return this;
        //}
        //public EcsQueryDesc WithNone<T0>() where T0 : struct => WithNone(compIds: world.GetComponentId<T0>());
        //public EcsQueryDesc WithNone<T0, T1>() where T0 : struct where T1 : struct => WithNone(world.GetComponentId<T0>(), world.GetComponentId<T1>());
        //public EcsQueryDesc WithNone<T0, T1, T2>() where T0 : struct where T1 : struct where T2 : struct => WithNone(world.GetComponentId<T0>(), world.GetComponentId<T1>(), world.GetComponentId<T2>());
        #endregion
    }
}
