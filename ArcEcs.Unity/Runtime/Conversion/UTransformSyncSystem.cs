using UnityEngine;

namespace Poly.ArcEcs.Unity
{
    public class UTransformSyncToSystem : IEcsSystem
    {
        private EcsQuery transformQuery;
        public void Init(EcsWorld world)
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
    public class UTransformSyncFromSystem : IEcsSystem
    {
        private EcsQuery transformQuery;
        public void Init(EcsWorld world)
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
