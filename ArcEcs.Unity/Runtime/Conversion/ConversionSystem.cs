using System;
using UnityEngine.SceneManagement;

namespace Poly.ArcEcs.Unity
{
    public class ConversionSystem : ISystem, IDisposable
    {
        private World world;
        private Query query;

        public ConversionSystem()
        {
        }
        public void Init(World world)
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
