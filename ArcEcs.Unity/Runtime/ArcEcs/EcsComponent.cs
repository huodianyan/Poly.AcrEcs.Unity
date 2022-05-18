using System;
using System.Runtime.CompilerServices;

namespace Poly.ArcEcs
{
    #region Component
    public delegate IEcsComponentArray CreateComponentArrayDelegate(int capacity);
    internal delegate void CopyComponentDelegate(EcsArchetype src, int chunkId, EcsArchetype dest);
    internal struct EcsComponentType
    {
        //255: 0~254(0xFE)
        internal byte Id;
        internal Type Type;
        internal CreateComponentArrayDelegate ComponentArrayCreator;
        internal CopyComponentDelegate CopyChunkComponent;
    }

    public interface IEcsComponentArray
    {
        int Capacity { get; }
        int Count { get; }

        void Add();
        object Get(int index);
        void Set(int index, object comp);
        //void Clear();
        bool RemoveAt(int index);
    }
    public class EcsComponentArray<T> : IEcsComponentArray where T : struct
    {
        private T[] items;
        private int count;

        public int Capacity => items.Length;
        public int Count => count;
        public T[] Array => items;

        public EcsComponentArray(int capacity)
        {
            items = new T[capacity];
            count = 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add() => Add(default);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Add(T item)
        {
            if (count == items.Length) System.Array.Resize(ref items, count << 1);
            items[count++] = item;
            return count - 1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveAt(int index)
        {
            var result = false;
            if (--count > index)
            {
                items[index] = items[count];
                result = true;
            }
            items[count] = default;
            return result;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object IEcsComponentArray.Get(int index) => items[index];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IEcsComponentArray.Set(int index, object comp) => items[index] = (T)comp;
    }
    #endregion
}
