using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Poly.ArcEcs.Unity
{
    public class GOEntity : MonoBehaviour
    {
        internal EcsWorld world;
        internal EcsEntity entity;
        // internal EConvertMode convertMode;

        public EcsWorld World => world;
        public EcsEntity Entity => entity;

        private void OnDestroy()
        {
            if (world != null && !world.IsDisposed && world.IsEntityValid(entity))
                world.DestroyEntity(entity);
        }
    }
}