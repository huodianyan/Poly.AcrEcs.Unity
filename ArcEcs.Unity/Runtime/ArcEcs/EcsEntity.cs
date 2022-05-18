namespace Poly.ArcEcs
{
    #region Entity
    //public struct EcsEntity
    //{
    //    public int Index;
    //    public int Version;
    //}
    public struct EcsEntity
    {
        internal int Index;
        internal short Version;
        internal byte ComponentCount;
        internal int ArchetypeId;
        internal int ArchetypeChunkId;
    }
    #endregion
}
