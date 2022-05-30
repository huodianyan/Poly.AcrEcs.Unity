using System;
using System.Collections.Generic;

namespace Poly.ArcEcs
{
    public class EcbSystem : ISystem
    {
        private EntityCommandBuffer ecb;
        public void Init(World world)
        {
            ecb = new EntityCommandBuffer(world);
        }
        public void Update()
        {
            ecb.Execute();
        }

        public EntityCommandBuffer CreateEcb()
        {
            return ecb;
        }
    }
}
