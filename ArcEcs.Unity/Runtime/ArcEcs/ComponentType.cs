using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Poly.ArcEcs
{
    public struct Parent
    {
        public Entity Entity;
    }
    public struct Children
    {
        public List<Entity> EntityList;

        public void AddChild(Entity entity)
        {
            EntityList ??= new List<Entity>();
            if (!EntityList.Contains(entity))
                EntityList.Add(entity);
        }
        public void RemoveChild(Entity entity)
        {
            EntityList.Remove(entity);
        }
    }

    public delegate IComponentArray CreateComponentArrayDelegate(int capacity);
    internal delegate void CopyComponentDelegate(Archetype src, int chunkId, Archetype dest);
    internal abstract class ComponentType
    {
        //255: 0~254(0xFE)
        internal readonly byte Id;
        //internal object Default;
        internal IComponentArray CompArray;
        //internal CreateComponentArrayDelegate ComponentArrayCreator;
        //internal CopyComponentDelegate CopyChunkComponent;
        internal Type Type;

        internal ComponentType(byte compId)
        {
            Id = compId;
        }
        internal abstract IComponentArray CreateComponentArray(int capacity);
        internal abstract void MoveChunkComponent(Archetype src, int chunkId, Archetype dest);
        //internal abstract void DestroyComponent(Archetype src, int chunkId);
    }
    internal class ComponentType<T> : ComponentType where T : struct
    {
        internal ComponentType(byte compId) : base(compId)
        {
            Type = typeof(T);
            CompArray = new ComponentArray<T>(1);
        }
        internal override IComponentArray CreateComponentArray(int capacity)
        {
            return new ComponentArray<T>(capacity);
        }
        internal override void MoveChunkComponent(Archetype src, int chunkId, Archetype dest)
        {
            src.MoveComponent<T>(Id, chunkId, dest);
        }
        //internal override void DestroyComponent(Archetype archetype, int chunkId)
        //{
        //    archetype.RemoveComponent(chunkId);
        //}
    }
}
