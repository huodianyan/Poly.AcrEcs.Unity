using System;
using System.Collections.Generic;

namespace Poly.Collections
{
    public interface IArrayPool<T>
    {
        T[] Rent(int count);
        void Return(T[] array);
    }
    public class ArrayPool<T> : IArrayPool<T>
    {
        private static readonly IArrayPool<T> shared = new ArrayPool<T>();
        public static IArrayPool<T> Shared => shared;

        private object lockObj = new object();
        private Queue<T[]>[] arrayPools = null;
        private readonly int maxPower;
        private readonly bool isValueType;

        public ArrayPool(int maxPower = 32)
        {
            this.maxPower = maxPower;
            isValueType = typeof(T).IsValueType;
            arrayPools = new Queue<T[]>[maxPower];
        }
        public T[] Rent(int count)
        {
            var power = 1;
            var num = 2;
            while (num < count)
            {
                power++;
                num <<= 1;
            }
            lock (lockObj)
            {
                var pool = arrayPools[power];
                if (pool != null && pool.Count > 0)
                    return pool.Dequeue();
            }
            return new T[num];
        }
        public void Return(T[] array)
        {
            var count = array.Length;
            var power = 1;
            var num = 2;
            while (num < count)
            {
                power++;
                num <<= 1;
            }
            if(!isValueType) Array.Clear(array, 0, count);
            lock (lockObj)
            {
                var pool = arrayPools[power - 1];
                if (pool == null)
                {
                    pool = new Queue<T[]>();
                    arrayPools[power] = pool;
                }
                pool.Enqueue(array);
            }
        }
    }
}
