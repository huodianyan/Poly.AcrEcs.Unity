//using System.Collections.Generic;

//namespace Poly.Collections
//{
//    public interface IByteArrayPool
//    {
//        byte[] Rent(int count);
//        void Return(byte[] array);
//    }
//    public class ByteArrayPool : IByteArrayPool
//    {
//        private static IByteArrayPool shared = new ByteArrayPool();
//        public static IByteArrayPool Shared => shared;

//        private Queue<byte[]>[] arrayPools = null;
//        private readonly int maxPower;

//        public ByteArrayPool(int maxPower = 32)
//        {
//            this.maxPower = maxPower;
//            arrayPools = new Queue<byte[]>[maxPower];
//        }
//        public byte[] Rent(int count)
//        {
//            var power = 1;
//            var num = 2;
//            while(num < count)
//            {
//                power++;
//                num <<= 1;
//            }
//            var pool = arrayPools[power];
//            if(pool != null && pool.Count > 0)
//                return pool.Dequeue();
//            return new byte[num];
//        }
//        public void Return(byte[] array)
//        {
//            var count = array.Length;
//            var power = 1;
//            var num = 2;
//            while (num < count)
//            {
//                power++;
//                num <<= 1;
//            }
//            var pool = arrayPools[power - 1];
//            if(pool == null)
//            {
//                pool = new Queue<byte[]>();
//                arrayPools[power] = pool;
//            }
//            pool.Enqueue(array);
//        }
//    }
//}
