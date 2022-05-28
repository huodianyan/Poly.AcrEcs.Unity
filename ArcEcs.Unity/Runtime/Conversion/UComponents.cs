using System.Collections;
using System.Collections.Generic;
using System.Text;
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


}