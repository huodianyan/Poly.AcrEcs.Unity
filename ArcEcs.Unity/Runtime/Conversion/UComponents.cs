using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Poly.Collections;
using UnityEngine;

namespace Poly.ArcEcs.Unity
{
    public struct UGameObject
    {
        public GameObject GameObject;
    }
    public struct UTransform
    {
        public Transform Transform;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public static implicit operator UTransform(Transform v) => new UTransform()
        {
            Transform = v,
            Position = v.localPosition,
            Rotation = v.localRotation,
            Scale = v.localScale
        };
    }
    public struct USceneObject
    {
        public string SceneId;
    }

    #region Physics
    public struct URigidbody
    {
        public Rigidbody Rigidbody;
        // public float drag;
        // public float angularDrag;
        // public float mass;
        // public bool useGravity;
        // public bool isKinematic;

        public static implicit operator URigidbody(Rigidbody v) => new URigidbody() { Rigidbody = v };
    }
    public struct UCollisionEnterEvent : IDisposable
    {
        public FastArray<Collision> Collisions;
        // public Entity Entity;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => Collisions.Dispose();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(Collision collision)
        {
            if (!Collisions.IsCreated) Collisions = new FastArray<Collision>(4);
            Collisions[Collisions.Length++] = collision;
        }
    }
    public struct UCollisionExitEvent : IDisposable
    {
        public FastArray<Collision> Collisions;
        // public Entity Entity;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => Collisions.Dispose();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(Collision collision)
        {
            if (!Collisions.IsCreated) Collisions = new FastArray<Collision>(4);
            Collisions[Collisions.Length++] = collision;
        }
    }
    #endregion
}