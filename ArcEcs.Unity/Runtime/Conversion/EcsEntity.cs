using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Poly.ArcEcs.Unity
{
    public class EcsEntity : MonoBehaviour
    {
        internal World world;
        internal Entity entity;
        // internal EConvertMode convertMode;

        public World World => world;
        public Entity Entity => entity;

        private void OnDestroy()
        {
            if (world != null && !world.IsDisposed && world.IsEntityValid(entity))
                world.DestroyEntity(entity);
        }
    }
}