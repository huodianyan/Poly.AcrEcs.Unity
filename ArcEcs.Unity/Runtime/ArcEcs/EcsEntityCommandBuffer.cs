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
        private readonly EcsEntityCommandBuffer ecb;
        private readonly EEcbCommandType commandType;
        private readonly EcsEntity entity;
        //public Type CompType;
        private readonly byte compId;
        private readonly int compIndex;
        private readonly EcsComponentArray<T> compArray;

        public EcbCommand(EcsEntityCommandBuffer ecb, EcsEntity entity, EEcbCommandType commandType, byte compId, EcsComponentArray<T> compArray, int compIndex)
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

    public class EcsEntityCommandBuffer : IDisposable
    {
        internal readonly EcsWorld world;
        //internal IEcsComponentArray[] compArrays;
        private Dictionary<byte, IEcsComponentArray> compArrayDict;
        private IEcbCommand[] commands;
        private int commandCount;

        public EcsEntityCommandBuffer(EcsWorld world)
        {
            this.world = world;
            commands = new IEcbCommand[32];
            compArrayDict = new Dictionary<byte, IEcsComponentArray>();
        }
        public void Dispose()
        {
            commands = null;
            //foreach(var compArray in compArrayDict.Values) compArray.
            compArrayDict.Clear();
            compArrayDict = null;
        }

        private EcsComponentArray<T> GetCompArray<T>(byte compId) where T : struct
        {
            if (compArrayDict.TryGetValue(compId, out var compArray))
                return (EcsComponentArray<T>)compArray;
            var compArray1 = new EcsComponentArray<T>(32);
            compArrayDict[compId] = compArray1;
            return compArray1;
        }
        private EcbCommand<T> CreateCommand<T>(EcsEntity entity, EEcbCommandType commandType, T comp) where T : struct
        {
            if (commandCount == commands.Length) Array.Resize(ref commands, commandCount << 1);
            var compId = world.GetComponentId<T>();
            var compArray = GetCompArray<T>(compId);
            var compIndex = 0;
            if (commandType != EEcbCommandType.Remove)
                compIndex = compArray.Add(comp);
            var cmd = new EcbCommand<T>(this, entity, commandType, compId, compArray, compIndex);
            //cmd.Entity = entity;
            //cmd.CommandType = commandType;
            //cmd.CompType = typeof(T);
            //cmd.CompId = world.GetComponentId<T>();

            commands[commandCount++] = cmd;
            return cmd;
        }
        public void AddComponent<T>(EcsEntity entity, T comp) where T : struct
        {
            CreateCommand<T>(entity, EEcbCommandType.Add, comp);
        }
        public void SetComponent<T>(EcsEntity entity, T comp) where T : struct
        {
            CreateCommand<T>(entity, EEcbCommandType.Set, comp);
        }
        public void RemoveComponent<T>(EcsEntity entity) where T : struct
        {
            CreateCommand<T>(entity, EEcbCommandType.Remove, default);
        }
        public void Execute()
        {
            for (int i = 0; i < commandCount; i++)
            {
                ref var cmd = ref commands[i];
                cmd.Execute();
            }
            Array.Clear(commands, 0, commandCount);
            commandCount = 0;
            foreach (var compArray in compArrayDict.Values)
                compArray.Clear();
        }
    }

    public class EcbSystem : IEcsSystem
    {
        private EcsEntityCommandBuffer ecb;
        public void Init(EcsWorld world)
        {
            ecb = new EcsEntityCommandBuffer(world);
        }
        public void Update()
        {
            ecb.Execute();
        }

        public EcsEntityCommandBuffer CreateEcb()
        {
            return ecb;
        }
    }
    //public partial class EcsWorld
    //{
    //    public EcsEntityCommandBuffer Get
    //}
}
