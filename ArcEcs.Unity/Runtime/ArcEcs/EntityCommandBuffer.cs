using System;
using System.Collections.Generic;

namespace Poly.ArcEcs
{
    public class EntityCommandBuffer : IDisposable
    {
        internal readonly World world;
        //internal IEcsComponentArray[] compArrays;
        private Dictionary<byte, IComponentArray> compArrayDict;
        private IEcbCommand[] commands;
        private int commandCount;

        public EntityCommandBuffer(World world)
        {
            this.world = world;
            commands = new IEcbCommand[32];
            compArrayDict = new Dictionary<byte, IComponentArray>();
        }
        public void Dispose()
        {
            commands = null;
            //foreach(var compArray in compArrayDict.Values) compArray.
            compArrayDict.Clear();
            compArrayDict = null;
        }
        private ComponentArray<T> GetCompArray<T>(byte compId) where T : struct
        {
            if (compArrayDict.TryGetValue(compId, out var compArray))
                return (ComponentArray<T>)compArray;
            var compArray1 = new ComponentArray<T>(32);
            compArrayDict[compId] = compArray1;
            return compArray1;
        }
        private EcbCommand<T> CreateCommand<T>(Entity entity, EEcbCommandType commandType, T comp) where T : struct
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
        public void AddComponent<T>(Entity entity, T comp) where T : struct
        {
            CreateCommand<T>(entity, EEcbCommandType.Add, comp);
        }
        public void SetComponent<T>(Entity entity, T comp) where T : struct
        {
            CreateCommand<T>(entity, EEcbCommandType.Set, comp);
        }
        public void RemoveComponent<T>(Entity entity) where T : struct
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
}
