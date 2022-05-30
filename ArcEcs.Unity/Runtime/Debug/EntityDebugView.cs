#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poly.ArcEcs.Unity
{
    public class EntityDebugView : MonoBehaviour
    {
        private static Type[] typesCache;

        [NonSerialized]
        public World World;
        [NonSerialized]
        public Entity Entity;
        [NonSerialized]
        public WorldDebugSystem DebugSystem;
        [SerializeReference]
        public List<object> ComponentList = new List<object>();

        internal void Init(World world, Entity entity, WorldDebugSystem system)
        {
            this.World = world;
            this.Entity = entity;
            this.DebugSystem = system;
            UpdateName();
        }
        // internal void Release()
        // {
        //     World = null;
        //     DebugSystem = null;
        // }
        internal void UpdateName()
        {
            if (World == null) return;
            var entityName = $"{Entity.Index.ToString("X8")}[{Entity.Version}]";
            if (Entity.Version > 0)
            {
                var compCount = World.GetComponentTypes(Entity, ref typesCache);
                for (var i = 0; i < compCount; i++)
                {
                    entityName = $"{entityName}:{typesCache[i].Name}";
                }
            }
            name = entityName;
        }
    }
    // public static class EditorExtensions
    // {
    //     public static string GetCleanGenericTypeName(Type type)
    //     {
    //         if (!type.IsGenericType)
    //         {
    //             return type.Name;
    //         }
    //         var constraints = "";
    //         foreach (var constraint in type.GetGenericArguments())
    //         {
    //             constraints += constraints.Length > 0 ? $", {GetCleanGenericTypeName(constraint)}" : constraint.Name;
    //         }
    //         var genericIndex = type.Name.LastIndexOf("`", StringComparison.Ordinal);
    //         var typeName = genericIndex == -1
    //             ? type.Name
    //             : type.Name.Substring(0, genericIndex);
    //         return $"{typeName}<{constraints}>";
    //     }
    // }
}
#endif