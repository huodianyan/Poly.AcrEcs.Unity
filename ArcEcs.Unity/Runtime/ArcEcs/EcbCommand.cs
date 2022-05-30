using System;
using System.Collections.Generic;

namespace Poly.ArcEcs
{
    internal enum EEcbCommandType : byte
    {
        Add,
        Remove,
        Set,
        SetOrAdd
    }
    internal interface IEcbCommand
    {
        void Execute();
    }
    internal struct EcbCommand<T> : IEcbCommand where T : struct
    {
        private readonly EntityCommandBuffer ecb;
        private readonly EEcbCommandType commandType;
        private readonly Entity entity;
        //public Type CompType;
        private readonly byte compId;
        private readonly int compIndex;
        private readonly ComponentArray<T> compArray;

        public EcbCommand(EntityCommandBuffer ecb, Entity entity, EEcbCommandType commandType, byte compId, ComponentArray<T> compArray, int compIndex)
        {
            this.ecb = ecb;
            this.entity = entity;
            this.commandType = commandType;
            this.compId = compId;
            this.compArray = compArray;
            this.compIndex = compIndex;
        }
        public void Execute()
        {
            var world = ecb.world;
            ref var comp = ref compArray.Array[compIndex];
            if (commandType == EEcbCommandType.Add)
            {
                world.AddComponent(entity, comp);
            }
            else if (commandType == EEcbCommandType.Set)
            {
                world.SetComponent(entity, comp);
            }
            else if (commandType == EEcbCommandType.Remove)
            {
                world.RemoveComponent(entity, compId);
            }
        }
    }
}
