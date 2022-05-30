using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Poly.ArcEcs
{
    public interface IComponentArray
    {
        int Capacity { get; }
        int Count { get; }

        void Add();
        object Get(int index);
        void Set(int index, object comp);
        void Clear();
        bool RemoveAt(int index, bool isDispose = false);
    }
    public class ComponentArray<T> : IComponentArray where T : struct
    {
        private T[] items;
        private int count;

        public int Capacity => items.Length;
        public int Count => count;
        public T[] Array => items;

        public ComponentArray(int capacity)
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
        public bool RemoveAt(int index, bool isDispose = false)
        {
            var result = false;
            if (isDispose)
                if (items[index] is IDisposable disposable) disposable.Dispose();
            if (--count > index)
            {
                items[index] = items[count];
                result = true;
            }
            items[count] = default;
            return result;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object IComponentArray.Get(int index) => items[index];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IComponentArray.Set(int index, object comp) => items[index] = (T)comp;
        public void Clear()
        {
            System.Array.Clear(items, 0, count);
            count = 0;
        }
    }
}
