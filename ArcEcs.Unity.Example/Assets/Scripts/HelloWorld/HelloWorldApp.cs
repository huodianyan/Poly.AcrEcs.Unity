using System;
using System.Collections.Generic;
using UnityEngine;

namespace Poly.ArcEcs.Unity.Example
{
    public class HelloWorldApp : MonoBehaviour
    {
        [SerializeField]
        private World.Config config;

        private World world;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            world = new World("Test", null, config);

            // world.RegisterComponent<CompA>();
            // world.RegisterComponent<CompB>();
            // world.RegisterComponent<CompC>();

            //PreUpdate
            world.AddSystem(new ConversionSystem());
            world.AddSystem(new UTransformSyncFromSystem());
            //BeginUpdateEcbSystem
            world.AddSystem(new BeginUpdateEcbSystem());

            world.AddSystem(new TestQueryEventSystem());

            //LateUpdate
            world.AddSystem(new UTransformSyncToSystem());
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
            Entity entity = default;
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
    public class TestQueryEventSystem : ISystem, IDisposable
    {
        private Query query;

        public TestQueryEventSystem()
        {
        }
        public void Init(World world)
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
            query.ForEach((Entity entity, ref CompA a) =>
            {
                a.A++;
            });
        }
        private void OnArchetypeAddedEvent(Archetype archetype)
        {
            Debug.Log($"{query.QueryDesc}.Add: {archetype.Id}");
        }
    }
}