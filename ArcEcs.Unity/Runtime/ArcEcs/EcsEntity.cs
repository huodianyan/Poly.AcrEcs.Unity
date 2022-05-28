using System;
using System.Runtime.CompilerServices;

namespace Poly.ArcEcs
{
    [Serializable]
    public struct EcsEntity : IEquatable<EcsEntity>
    {
        public int Index;
        public short Version;

        //public EcsEntity(EcsEntityInternal e)
        //{
        //    Index = e.Index;
        //    Version = e.Version;
        //}
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValid() => Version > 0;
        public bool Equals(EcsEntity other) => Index == other.Index;// && Version == other.Version;
        //public static implicit operator EcsEntity(EcsEntityInternal v) => new EcsEntity { Index = v.Index, Version = v.Version };
        public override int GetHashCode() => Index;
    }
    internal struct EcsEntityInternal
    {
        public int Index;
        public short Version;
        internal byte ComponentCount;
        internal int ArchetypeId;
        internal int ArchetypeChunkId;

        public override string ToString()
        {
            return $"EntityInternal{{{Index},{ComponentCount},{ArchetypeId},{ArchetypeChunkId}}}";
        }

        public static implicit operator EcsEntity(EcsEntityInternal v) => new EcsEntity { Index = v.Index, Version = v.Version };
    }
}
