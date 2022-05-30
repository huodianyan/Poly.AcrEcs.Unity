using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Poly.ArcEcs.Unity
{
    public static class ArcEcsUnityExtensions
    {
        private static List<Component> compList = new List<Component>();
        private static StringBuilder sb = new StringBuilder();
        public static Entity CreateEntity(this World world, GameObject go)
        {
            var entity = world.CreateEntity(new UGameObject { GameObject = go });
            //GOEntity
            var goe = go.AddComponent<EcsEntity>();
            goe.world = world;
            goe.entity = entity;
            //Transform
            var transform = go.transform;
            world.AddComponent<UTransform>(entity, transform);
            // world.AddComponent<Parent>(entity);
            // world.AddComponent<Children>(entity);
            // if (transform.parent != null)
            // {
            //     var parentGOE = transform.parent.GetComponent<GOEntity>();
            //     world.SetComponent<Parent>(entity, new Parent { Entity = parentGOE.Entity });
            //     // parent.Entity = parentGOE.Entity;
            //     ref var children = ref world.GetComponent<Children>(parentGOE.Entity);
            //     children.AddChild(entity);
            // }

            compList.Clear();
            go.GetComponents<Component>(compList);
            foreach (var comp in compList)
            {
                if (comp is Transform) continue;
                else if (comp is IEcsComponent ecsComponent)
                    ecsComponent.ApplyTo(world, entity);
                else if (comp is Rigidbody rigidbody)
                    world.AddComponent<URigidbody>(entity, rigidbody);
            }
            for (int i = 0; i < go.transform.childCount; i++)
                CreateEntity(world, go.transform.GetChild(i).gameObject);
            return entity;
        }
        public static string GetCompnentsName(this World world, byte[] compIds, int count)
        {
            if (compIds == null || count == 0) return "";
            sb.Clear();
            for (int i = 0; i < count; i++)
            {
                if (i > 0) sb.Append("|");
                sb.Append(world.GetComponentType(compIds[i]).Name);
            }
            return sb.ToString();
        }

    }
}