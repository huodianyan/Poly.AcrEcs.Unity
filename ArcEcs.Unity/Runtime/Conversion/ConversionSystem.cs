using System;
using UnityEngine.SceneManagement;

namespace Poly.ArcEcs.Unity
{
    public class ConversionSystem : IEcsSystem, IDisposable
    {
        private EcsWorld world;
        private EcsQuery query;

        public ConversionSystem()
        {
        }
        public void Init(EcsWorld world)
        {
            this.world = world;

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }
        public void Dispose()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        public void Update()
        {
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            var gos = scene.GetRootGameObjects();
            foreach (var go in gos)
            {
                world.CreateEntity(go);
            }
        }
        private void OnSceneUnloaded(Scene scene)
        {

        }
    }

}
