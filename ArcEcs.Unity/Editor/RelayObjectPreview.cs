using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Poly.ArcEcs.Unity.Editor
{
    [CustomEditor(typeof(EntityDebugView))]
    sealed class EntityDebugViewInspector : UnityEditor.Editor
    {
        static object[] compCache = new object[32];
        SerializedProperty compListProp;


        private void OnEnable()
        {
            // Setup the SerializedProperties.
            compListProp = serializedObject.FindProperty("ComponentList");

            var debugView = (EntityDebugView)target;
            if (debugView.World == null)
                return;
            if (!debugView.gameObject.activeSelf)
                return;

            var world = debugView.World;
            var entity = debugView.Entity;
            var compList = debugView.ComponentList;
            compList.Clear();

            var count = world.GetComponents(debugView.Entity, ref compCache);
            for (var i = 0; i < count; i++)
            {
                var component = compCache[i];
                compList.Add(component);
                // var type = component.GetType();
                // var pool = world.GetComponentArrayType(type);
                // compList.Add(pool.Get(entity));
            }
        }

        public override void OnInspectorGUI()
        {
            var debugView = (EntityDebugView)target;
            if (debugView.World == null)
                return;
            if (!debugView.gameObject.activeSelf)
                return;

            var world = debugView.World;
            var entity = debugView.Entity;
            var compList = debugView.ComponentList;

            // compList.Clear();

            // var count = world.GetEntityComponents(debugView.Entity, ref compCache);
            // for (var i = 0; i < count; i++)
            // {
            //     var component = compCache[i];
            //     // compCache[i] = null;
            //     var type = component.GetType();
            //     var pool = world.GetComponentArrayType(type);
            //     compList.Add(pool.Get(entity));
            // }

            // compListProp = serializedObject.FindProperty("ComponentList");
            int changeIndex = -1;
            for (int i = 0; i < compListProp.arraySize; i++)
            {
                var prop = compListProp.GetArrayElementAtIndex(i);
                var component = compCache[i];
                var type = component.GetType();
                var propLabel = new GUIContent(type.Name);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(prop, propLabel, true);
                if (EditorGUI.EndChangeCheck())
                {
                    // Debug.Log($"{i}: {compList[i]}");
                    changeIndex = i;
                    // serializedObject.ApplyModifiedProperties();
                    // var pool = world.GetComponentArrayType(type);
                    // // pool.Set(debugView.Entity, compList[changeIndex]);
                    // try
                    // {
                    //     pool.Set(debugView.Entity, compList[i]);
                    // }
                    // catch (Exception ex)
                    // {
                    //     Debug.LogError($"{i}: {ex}");
                    // }
                }
            }
            if (changeIndex >= 0)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);

                var component = compCache[changeIndex];
                // var type = component.GetType();
                world.SetComponent(debugView.Entity, component);
                // var pool = world.GetComponentArrayType(type);
                // // pool.Set(debugView.Entity, compList[changeIndex]);
                // try
                // {
                //     pool.Set(debugView.Entity, compList[changeIndex]);
                // }
                // catch (Exception ex)
                // {
                //     Debug.LogError($"{changeIndex}: {ex}");
                // }
            }

            // EditorUtility.SetDirty(target);

            // base.OnInspectorGUI();
        }
    }


}
