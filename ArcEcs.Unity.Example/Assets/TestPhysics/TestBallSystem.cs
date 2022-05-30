using System;
using System.Collections.Generic;
using UnityEngine;

namespace Poly.ArcEcs.Unity.Example
{
    public class TestBallSystem : ISystem, IDisposable
    {
        private Query collisionEnterQuery;
        public void Init(World world)
        {
            var desc = world.CreateQueryDesc().WithAll<TestBall, URigidbody, UCollisionEnterEvent>().Build();
            collisionEnterQuery = world.GetQuery(desc);
        }
        public void Dispose()
        {
        }
        public void Update()
        {
            collisionEnterQuery.ForEach((ref URigidbody uRigid, ref UCollisionEnterEvent enterEvent) =>
            {
                var rigidbody = uRigid.Rigidbody;
                var count = enterEvent.Collisions.Length;
                if (count == 0) return;
                foreach (var collision in enterEvent.Collisions)
                {
                    Debug.Log($"TestBallSystem: {collision.contactCount}");
                    if (collision.contactCount > 0)
                    {
                        var contact = collision.GetContact(0);
                        rigidbody.AddForceAtPosition(Vector3.left * 10, contact.point, ForceMode.Force);
                    }
                }
            });
        }
    }
}