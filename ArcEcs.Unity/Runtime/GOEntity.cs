using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Poly.ArcEcs.Unity
{
    // public enum EConvertMode
    // {
    //     ConvertAndDestroy,
    //     ConvertAndSave
    // }
    public class GOEntity : MonoBehaviour
    {
        internal EcsWorld world;
        internal EcsEntity entity;
        // internal EConvertMode convertMode;

        public EcsWorld World => world;
        public EcsEntity Entity => entity;

        private void OnDestroy()
        {
            if (world != null && world.IsEntityValid(entity))
                world.DestroyEntity(entity);
        }
    }

    // public interface IGOEntityComponent
    // {
    //     void Apply(EcsWorld world, int entityId);
    // }
    public struct UGameObject
    {
        public GameObject GameObject;
    }
    public struct UTransform
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public static implicit operator UTransform(Transform v) => new UTransform()
        {
            Position = v.localPosition,
            Rotation = v.localRotation,
            Scale = v.localScale
        };
    }
    public struct USceneObject
    {
        public string SceneId;
    }

    public static class ArcEcsExtensions
    {
        private static List<Component> compList = new List<Component>();
        private static StringBuilder sb = new StringBuilder();
        public static EcsEntity CreateEntity(this EcsWorld world, GameObject go)
        {
            var entity = world.CreateEntity(new UGameObject { GameObject = go });
            //GOEntity
            var goe = go.AddComponent<GOEntity>();
            goe.world = world;
            goe.entity = entity;
            //Transform
            var transform = go.transform;
            world.AddComponent<UTransform>(entity, transform);
            world.AddComponent<Parent>(entity);
            world.AddComponent<Children>(entity);
            if (transform.parent != null)
            {
                var parentGOE = transform.parent.GetComponent<GOEntity>();
                world.SetComponent<Parent>(entity, new Parent { Entity = parentGOE.Entity });
                // parent.Entity = parentGOE.Entity;
                ref var children = ref world.GetComponent<Children>(parentGOE.Entity);
                children.AddChild(entity);
            }

            compList.Clear();
            go.GetComponents<Component>(compList);
            foreach (var comp in compList)
            {
                if (comp is Transform) continue;
            }
            for (int i = 0; i < go.transform.childCount; i++)
                CreateEntity(world, go.transform.GetChild(i).gameObject);
            return entity;
        }
        public static string GetCompnentsName(this EcsWorld world, byte[] compIds, int count)
        {
            if (compIds == null || count == 0) return "";
            sb.Clear();
            for (int i = 0; i < count; i++)
            {
                if(i > 0) sb.Append("|");
                sb.Append(world.GetComponentType(compIds[i]).Name);
            }
            return sb.ToString();
        }

    }
}