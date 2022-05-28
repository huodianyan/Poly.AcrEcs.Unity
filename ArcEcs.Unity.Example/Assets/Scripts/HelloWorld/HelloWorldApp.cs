using System.Collections.Generic;
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
            DontDestroyOnLoad(gameObject);
            world = new EcsWorld("Test", null, config);

            // world.RegisterComponent<CompA>();
            // world.RegisterComponent<CompB>();
            // world.RegisterComponent<CompC>();

            world.AddSystem(new ConversionSystem());
            world.AddSystem(new TestQueryEventSystem());
#if UNITY_EDITOR
            world.AddSystem(new WorldDebugSystem());
#endif
            world.Init();
        }
        private void OnDestroy()
        {
            world.Dispose();
        }
        private void Update()
        {
            EcsEntity entity = default;
            if (Input.GetKeyDown(KeyCode.A))
            {
                entity = world.CreateEntity();
                world.AddComponent<CompA>(entity);
                world.AddComponent<CompB>(entity);
                world.AddComponent<CompC>(entity);
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                entity = world.CreateEntity();
                world.AddComponent<CompB>(entity);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                entity = world.CreateEntity();
                world.AddComponent<CompC>(entity);
            }
            if (entity.IsValid())
                Debug.Log($"Create Entity: {entity}");
            world.Update();
        }
    }
    public struct CompA
    {
        public float A;
        public string Str;
        public List<int> IntList;
    }
    public struct CompB
    {
        public float B;
    }
    public struct CompC
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
            var desc = world.CreateQueryDesc().WithAll<CompA, CompB>().WithNone<CompC>().Build();
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
            query.ForEach((EcsEntity entity, ref CompA a) =>
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