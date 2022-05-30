using UnityEngine;

namespace Poly.ArcEcs.Unity
{
    public class UTransformSyncToSystem : ISystem
    {
        private Query transformQuery;
        public void Init(World world)
        {
            var desc = world.CreateQueryDesc().WithAll<UTransform>().Build();
            transformQuery = world.GetQuery(desc);
        }
        public void Update()
        {
            transformQuery.ForEach((ref UTransform utr) =>
            {
                var transform = utr.Transform;
                transform.localPosition = utr.Position;
                transform.localRotation = utr.Rotation;
                transform.localScale = utr.Scale;
            });
        }
    }
    public class UTransformSyncFromSystem : ISystem
    {
        private Query transformQuery;
        public void Init(World world)
        {
            var desc = world.CreateQueryDesc().WithAll<UTransform>().Build();
            transformQuery = world.GetQuery(desc);
        }
        public void Update()
        {
            transformQuery.ForEach((ref UTransform utr) =>
            {
                var transform = utr.Transform;
                // Debug.Log($"{GetType().Name}: {transform}");
                // if(transform == null) return;
                utr.Position = transform.localPosition;
                utr.Rotation = transform.localRotation;
                utr.Scale = transform.localScale;
            });
        }
    }
}
