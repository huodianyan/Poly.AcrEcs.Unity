using System;
using System.Collections.Generic;
using UnityEngine;

namespace Poly.ArcEcs.Unity.Example
{
    public class TestPhysicsApp : MonoBehaviour
    {
        [SerializeField]
        private World.Config config;

        private World world;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            world = new World("TestPhysics", null, config);

            //PreUpdate
            world.AddSystem(new ConversionSystem());
            world.AddSystem(new UTransformSyncFromSystem());
            //BeginUpdateEcbSystem
            world.AddSystem(new BeginUpdateEcbSystem());

            world.AddSystem(new TestBallSystem());

            //LateUpdate
            world.AddSystem(new UTransformSyncToSystem());
            world.AddSystem(new RemoveCompSystem());
#if UNITY_EDITOR
            world.AddSystem(new WorldDebugSystem());
#endif
            world.Init();

            var removeCompSystem = world.GetSystem<RemoveCompSystem>();
            removeCompSystem.AddComp<UCollisionEnterEvent>();
            removeCompSystem.AddComp<UCollisionExitEvent>();
        }
        private void OnDestroy()
        {
            world.Dispose();
        }
        private void Update()
        {
            world.Update();
        }
    }
}