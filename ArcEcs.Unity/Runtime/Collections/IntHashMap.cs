//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace Poly.Ecs
//{
//    public static class ArrayUtil
//    {
//        //https://www.geeksforgeeks.org/smallest-power-of-2-greater-than-or-equal-to-n/
//        public static int NextPowerOf2(int n)
//        {
//            n--;
//            n |= n >> 1;
//            n |= n >> 2;
//            n |= n >> 4;
//            n |= n >> 8;
//            n |= n >> 16;
//            n++;
//            return n;
//        }
//        //https://www.geeksforgeeks.org/highest-power-2-less-equal-given-number/
//        public static int PrePowerOf2(int x)
//        {
//            x |= x >> 1;
//            x |= x >> 2;
//            x |= x >> 4;
//            x |= x >> 8;
//            x |= x >> 16;
//            return x ^ (x >> 1);
//        }
//    }
//    [Serializable]
//    public sealed class IntHashMap<T> : IEnumerable<int>
//    {
//        public int length;
//        public int capacity;
//        public int capacityMinusOne;
//        public int lastIndex;
//        public int freeIndex;

//        public int[] buckets;

//        public T[] data;
//        public Slot[] slots;

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public IntHashMap(in int capacity = 0)
//        {
//            this.lastIndex = 0;
//            this.length = 0;
//            this.freeIndex = -1;

//            this.capacity = ArrayUtil.NextPowerOf2(capacity);
//            capacityMinusOne = this.capacity - 1;

//            this.buckets = new int[this.capacity];
//            this.slots = new Slot[this.capacity];
//            this.data = new T[this.capacity];
//        }

//        public struct Slot
//        {
//            public int key;
//            public int next;
//        }

//    }

//    public static class IntHashMapExtensions
//    {
//        public static bool Add<T>(this IntHashMap<T> hashMap, in int key, in T value, out int slotIndex)
//        {
//            var rem = key & hashMap.capacityMinusOne;

//            for (var i = hashMap.buckets[rem] - 1; i >= 0; i = hashMap.slots[i].next)
//            {
//                if (hashMap.slots[i].key - 1 == key)
//                {
//                    slotIndex = -1;
//                    return false;
//                }
//            }

//            if (hashMap.freeIndex >= 0)
//            {
//                slotIndex = hashMap.freeIndex;
//                hashMap.freeIndex = hashMap.slots[slotIndex].next;
//            }
//            else
//            {
//                if (hashMap.lastIndex == hashMap.capacity)
//                {
//                    var newCapacityMinusOne = HashHelpers.ExpandPrime(hashMap.length);
//                    var newCapacity = newCapacityMinusOne + 1;

//                    ArrayHelpers.Grow(ref hashMap.slots, newCapacity);
//                    ArrayHelpers.Grow(ref hashMap.data, newCapacity);

//                    var newBuckets = new int[newCapacity];

//                    for (int i = 0, len = hashMap.lastIndex; i < len; ++i)
//                    {
//                        ref var slot = ref hashMap.slots[i];

//                        var newResizeIndex = (slot.key - 1) & newCapacityMinusOne;
//                        slot.next = newBuckets[newResizeIndex] - 1;

//                        newBuckets[newResizeIndex] = i + 1;
//                    }

//                    hashMap.buckets = newBuckets;
//                    hashMap.capacity = newCapacity;
//                    hashMap.capacityMinusOne = newCapacityMinusOne;

//                    rem = key & hashMap.capacityMinusOne;
//                }

//                slotIndex = hashMap.lastIndex;
//                ++hashMap.lastIndex;
//            }

//            ref var newSlot = ref hashMap.slots[slotIndex];

//            newSlot.key = key + 1;
//            newSlot.next = hashMap.buckets[rem] - 1;

//            hashMap.data[slotIndex] = value;

//            hashMap.buckets[rem] = slotIndex + 1;

//            ++hashMap.length;
//            return true;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static void Set<T>(this IntHashMap<T> hashMap, in int key, in T value, out int slotIndex)
//        {
//            var rem = key & hashMap.capacityMinusOne;

//            for (var i = hashMap.buckets[rem] - 1; i >= 0; i = hashMap.slots[i].next)
//            {
//                if (hashMap.slots[i].key - 1 == key)
//                {
//                    hashMap.data[i] = value;
//                    slotIndex = i;
//                    return;
//                }
//            }

//            if (hashMap.freeIndex >= 0)
//            {
//                slotIndex = hashMap.freeIndex;
//                hashMap.freeIndex = hashMap.slots[slotIndex].next;
//            }
//            else
//            {
//                if (hashMap.lastIndex == hashMap.capacity)
//                {
//                    var newCapacityMinusOne = HashHelpers.ExpandPrime(hashMap.length);
//                    var newCapacity = newCapacityMinusOne + 1;

//                    ArrayHelpers.Grow(ref hashMap.slots, newCapacity);
//                    ArrayHelpers.Grow(ref hashMap.data, newCapacity);

//                    var newBuckets = new int[newCapacity];

//                    for (int i = 0, len = hashMap.lastIndex; i < len; ++i)
//                    {
//                        ref var slot = ref hashMap.slots[i];
//                        var newResizeIndex = (slot.key - 1) & newCapacityMinusOne;
//                        slot.next = newBuckets[newResizeIndex] - 1;

//                        newBuckets[newResizeIndex] = i + 1;
//                    }

//                    hashMap.buckets = newBuckets;
//                    hashMap.capacity = newCapacity;
//                    hashMap.capacityMinusOne = newCapacityMinusOne;

//                    rem = key & hashMap.capacityMinusOne;
//                }

//                slotIndex = hashMap.lastIndex;
//                ++hashMap.lastIndex;
//            }

//            ref var newSlot = ref hashMap.slots[slotIndex];

//            newSlot.key = key + 1;
//            newSlot.next = hashMap.buckets[rem] - 1;

//            hashMap.data[slotIndex] = value;

//            hashMap.buckets[rem] = slotIndex + 1;

//            ++hashMap.length;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static bool Remove<T>(this IntHashMap<T> hashMap, in int key, [CanBeNull] out T lastValue)
//        {
//            var rem = key & hashMap.capacityMinusOne;

//            int next;
//            int num = -1;
//            for (var i = hashMap.buckets[rem] - 1; i >= 0; i = next)
//            {
//                ref var slot = ref hashMap.slots[i];
//                if (slot.key - 1 == key)
//                {
//                    if (num < 0)
//                    {
//                        hashMap.buckets[rem] = slot.next + 1;
//                    }
//                    else
//                    {
//                        hashMap.slots[num].next = slot.next;
//                    }

//                    lastValue = hashMap.data[i];

//                    slot.key = -1;
//                    slot.next = hashMap.freeIndex;

//                    --hashMap.length;
//                    if (hashMap.length == 0)
//                    {
//                        hashMap.lastIndex = 0;
//                        hashMap.freeIndex = -1;
//                    }
//                    else
//                    {
//                        hashMap.freeIndex = i;
//                    }

//                    return true;
//                }

//                next = slot.next;
//                num = i;
//            }

//            lastValue = default;
//            return false;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static bool Has<T>(this IntHashMap<T> hashMap, in int key)
//        {
//            var rem = key & hashMap.capacityMinusOne;

//            int next;
//            for (var i = hashMap.buckets[rem] - 1; i >= 0; i = next)
//            {
//                ref var slot = ref hashMap.slots[i];
//                if (slot.key - 1 == key)
//                {
//                    return true;
//                }

//                next = slot.next;
//            }

//            return false;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static bool TryGetValue<T>(this IntHashMap<T> hashMap, in int key, [CanBeNull] out T value)
//        {
//            var rem = key & hashMap.capacityMinusOne;

//            int next;
//            for (var i = hashMap.buckets[rem] - 1; i >= 0; i = next)
//            {
//                ref var slot = ref hashMap.slots[i];
//                if (slot.key - 1 == key)
//                {
//                    value = hashMap.data[i];
//                    return true;
//                }

//                next = slot.next;
//            }

//            value = default;
//            return false;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static T GetValueByKey<T>(this IntHashMap<T> hashMap, in int key)
//        {
//            var rem = key & hashMap.capacityMinusOne;

//            int next;
//            for (var i = hashMap.buckets[rem] - 1; i >= 0; i = next)
//            {
//                ref var slot = ref hashMap.slots[i];
//                if (slot.key - 1 == key)
//                {
//                    return hashMap.data[i];
//                }

//                next = slot.next;
//            }

//            return default;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static T GetValueByIndex<T>(this IntHashMap<T> hashMap, in int index) => hashMap.data[index];

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static int GetKeyByIndex<T>(this IntHashMap<T> hashMap, in int index) => hashMap.slots[index].key;

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static int TryGetIndex<T>(this IntHashMap<T> hashMap, in int key)
//        {
//            var rem = key & hashMap.capacityMinusOne;

//            int next;
//            for (var i = hashMap.buckets[rem] - 1; i >= 0; i = next)
//            {
//                ref var slot = ref hashMap.slots[i];
//                if (slot.key - 1 == key)
//                {
//                    return i;
//                }

//                next = slot.next;
//            }

//            return -1;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static void CopyTo<T>(this IntHashMap<T> hashMap, T[] array)
//        {
//            int num = 0;
//            for (int i = 0, li = hashMap.lastIndex; i < li && num < hashMap.length; ++i)
//            {
//                if (hashMap.slots[i].key - 1 < 0)
//                {
//                    continue;
//                }

//                array[num] = hashMap.data[i];
//                ++num;
//            }
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static void Clear<T>(this IntHashMap<T> hashMap)
//        {
//            if (hashMap.lastIndex <= 0)
//            {
//                return;
//            }

//            Array.Clear(hashMap.slots, 0, hashMap.lastIndex);
//            Array.Clear(hashMap.buckets, 0, hashMap.capacity);
//            Array.Clear(hashMap.data, 0, hashMap.capacity);

//            hashMap.lastIndex = 0;
//            hashMap.length = 0;
//            hashMap.freeIndex = -1;
//        }
//    }
//}
