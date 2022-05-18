namespace Poly.ArcEcs
{
    public static class EcsUtil
    {
        //https://www.geeksforgeeks.org/smallest-power-of-2-greater-than-or-equal-to-n/
        public static int NextPowerOf2(int n)
        {
            n--;
            n |= n >> 1;
            n |= n >> 2;
            n |= n >> 4;
            n |= n >> 8;
            n |= n >> 16;
            n++;
            return n;
        }
        //https://www.geeksforgeeks.org/highest-power-2-less-equal-given-number/
        public static int PrePowerOf2(int x)
        {
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return x ^ (x >> 1);
        }
    }
}
