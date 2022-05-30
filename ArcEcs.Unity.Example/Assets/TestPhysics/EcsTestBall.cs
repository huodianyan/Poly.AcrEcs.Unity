using System;

namespace Poly.ArcEcs.Unity.Example
{
    [Serializable]
    public struct TestBall
    {
        public int Id;
        public float Radius;
    }
    public class EcsTestBall : AEcsComponent<TestBall>
    {
    }
}