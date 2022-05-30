using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Poly.Collections;
using UnityEngine;

namespace Poly.ArcEcs.Unity
{
    [Flags]
    public enum EPhysicsEventFlag : byte
    {
        None = 0,
        CollisionEnter = 0x01,
        CollisionStay = 0x02,
        CollisionExit = 0x04,
        TriggerEnter = 0x08,
        TriggerStay = 0x10,
        TriggerExit = 0x20,
    }
    [RequireComponent(typeof(Rigidbody))]
    public class EcsPhysicsEventAgent : MonoBehaviour
    {
        [SerializeField]
        private EPhysicsEventFlag flag;
        private EcsEntity _ecsEntity;
        public EcsEntity EcsEntity => _ecsEntity ??= GetComponent<EcsEntity>();
        private void OnEnable()
        {
            // ecsEntity ??= GetComponent<EcsEntity>();
            // Debug.Log($"EcsPhysicsEventAgent.OnEnable: {ecsEntity}");
        }
        public void AddFlag(EPhysicsEventFlag flag)
        {
            this.flag |= flag;
        }
        public void RemoveFlag(EPhysicsEventFlag flag)
        {
            this.flag &= ~flag;
        }
        private void OnCollisionEnter(Collision other)
        {
            var ecsEntity = EcsEntity;
            if (ecsEntity == null) return;
            if((flag & EPhysicsEventFlag.CollisionEnter) == 0) return;
            var world = ecsEntity.world;
            var entity = ecsEntity.entity;
            if (!world.HasComponent<UCollisionEnterEvent>(entity))
                world.AddComponent<UCollisionEnterEvent>(entity);
            ref var enterEvent = ref world.GetComponent<UCollisionEnterEvent>(entity);
            enterEvent.Add(other);
        }
        private void OnCollisionExit(Collision other)
        {
            var ecsEntity = EcsEntity;
            if (ecsEntity == null) return;
            if((flag & EPhysicsEventFlag.CollisionExit) == 0) return;
            var world = ecsEntity.world;
            var entity = ecsEntity.entity;
            if (!world.HasComponent<UCollisionExitEvent>(entity))
                world.AddComponent<UCollisionExitEvent>(entity);
            ref var exitEvent = ref world.GetComponent<UCollisionExitEvent>(entity);
            exitEvent.Add(other);
        }
    }
}