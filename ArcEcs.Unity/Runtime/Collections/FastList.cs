using System;
using System.Runtime.CompilerServices;

namespace Poly.Collections
{
    public interface IFastList
    {
        int Capacity { get; }
        bool IsEmpty { get; }
        int Count { get; }
        //object this[int index] { get; set; }

        void Add();
        //void Add(object item);
        void Clear();
        //bool Contains(object item);
        //int IndexOf(object item);
        //bool Remove(object item);
        //object RemoveAt(int index);
        bool RemoveAtSwap(int index);
    }
    public interface IFastList<T> : IFastList where T : struct
    {
        T this[int index] { get; set; }

        void Add(T item);
        bool Contains(T item);
        int IndexOf(T item);
        bool Remove(T item);
        T RemoveAt(int index);
        ref T ElementAt(int index);
    }
    public struct FastList<T> : IFastList<T> where T : struct// : IEnumerable<T>
    {
        private T[] items;
        private int count;

        public int Capacity => items.Length;
        public bool IsEmpty => count == 0;
        public int Count => count;

        public T this[int index]
        {
            get => index >= items.Length ? default : items[index];
            set
            {
                if (index > items.Length) EnsureCapacity(index);
                if (index >= count)
                    count = index + 1;
                items[index] = value;
            }
        }
        //object IFastList.this[int index] { get => this[index]; set => this[index] = (T)value; }

        public FastList(int capacity)
        {
            items = new T[capacity];
            count = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int capacity)
        {
            var length = items.Length;
            while (length < capacity) length <<= 1;
            Array.Resize(ref items, length);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add() => Add(default);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            if (count == items.Length) Array.Resize(ref items, count << 1);
            items[count++] = item;
            //Console.WriteLine($"FastList.Add: {count}, {item.ToString()}");
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //void IFastList.Add(object item)
        //{
        //    Add(item == null ? default : (T)item);
        //}
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(FastList<T> range)
        {
            var newCount = range.count + count;
            if (newCount > items.Length) EnsureCapacity(newCount);
            for (int i = 0, j = range.count; i < j; i++)
                items[count++] = range[i];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            if (count > 0) Array.Clear(items, 0, count);
            count = 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item)
        {
            for (var index = count - 1; index >= 0; --index)
                if (item.Equals(items[index]))
                    return true;
            return false;
        }
        //bool IFastList.Contains(object item)
        //{
        //    return Contains((T)item);
        //}
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item) => Array.IndexOf(items, item);
        //int IFastList.IndexOf(object item)
        //{
        //    return IndexOf((T)item);
        //}
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index < 0) return false;
            RemoveAt(index);
            return true;
        }
        //bool IFastList.Remove(object item)
        //{
        //    return Remove((T)item);
        //}
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T RemoveAt(int index)
        {
            var result = items[index];
            if (--count > index)
                items[index] = items[count];
            items[count] = default;
            return result;
        }
        //object IFastList.RemoveAt(int index)
        //{
        //    return RemoveAt(index);
        //}
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveAtSwap(int index)
        {
            var result = false;
            if (--count > index)
            {
                items[index] = items[count];
                result = true;
            }
            //Console.WriteLine($"FastList.RemoveAtSwap: {count}, {index}");
            items[count] = default;
            return result;
        }
        public ref T ElementAt(int index) => ref items[index];
        public Enumerator GetEnumerator() => new Enumerator(this);

        public struct Enumerator : IDisposable //IEnumerator<T>
        {
            private readonly FastList<T> bag;
            private volatile int index;
            public Enumerator(FastList<T> bag)
            {
                this.bag = bag;
                index = -1;
            }
            public T Current => bag[index];
            public bool MoveNext() => ++index < bag.Count;
            public void Dispose() { }
        }
    }
}
