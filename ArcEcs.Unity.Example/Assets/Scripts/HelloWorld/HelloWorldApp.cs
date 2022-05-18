using System.Collections;
using System.Collections.Generic;
using Poly.ArcEcs;
using Poly.ArcEcs.Unity;
using UnityEngine;

namespace Poly.ArcEcs.Unity.Example
{
    public class HelloWorldApp : MonoBehaviour
    {
        [SerializeField]
        private EcsWorld.Config config;

        private EcsWorld world;

        private void Awake()
        {
            world = new EcsWorld("Test", null, config);

            world.RegisterComponent<AComp>();
            world.RegisterComponent<BComp>();
            world.RegisterComponent<CComp>();

            world.AddSystem(new TestQueryEventSystem());
            world.AddSystem(new WorldDebugSystem());
            world.Init();

        }
        private void OnDestroy()
        {
            world.Destroy();
        }
        private void Update()
        {
            int entity = -1;
            if (Input.GetKeyDown(KeyCode.A))
            {
                entity = world.CreateEntity();
                world.AddComponent<AComp>(entity);
                world.AddComponent<BComp>(entity);
                world.AddComponent<CComp>(entity);
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                entity = world.CreateEntity();
                world.AddComponent<BComp>(entity);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                entity = world.CreateEntity();
                world.AddComponent<CComp>(entity);
            }
            if (entity >= 0)
                Debug.Log($"Create Entity: {entity}");
            world.Update();
        }
    }
    public struct AComp
    {
        public float A;
        public string Str;
        public List<int> IntList;
    }
    public struct BComp
    {
        public float B;
    }
    public struct CComp
    {
        public float C;
    }
    public class TestQueryEventSystem : IEcsSystem
    {
        private EcsQuery query;

        public TestQueryEventSystem()
        {
        }
        public void Init(EcsWorld world)
        {
            var desc = world.CreateQueryDesc().WithAll<AComp>().Build();
            query = world.GetQuery(desc);
            query.ArchetypeAddedEvent += OnArchetypeAddedEvent;
        }
        public void Dispose()
        {
            query.ArchetypeAddedEvent -= OnArchetypeAddedEvent;
            query = null;
        }
        public void Update()
        {
            query.ForEach((int entityId, ref AComp a) =>
            {
                a.A++;
            });
        }
        private void OnArchetypeAddedEvent(EcsArchetype archetype)
        {
            Debug.Log($"{query.QueryDesc}.Add: {archetype.Id}");
        }
    }
}