using System;
using UnityEditor;
using UnityEngine;

namespace Poly.ArcEcs.Unity.Editor
{
    [CustomEditor(typeof(ArchetypeDebugView))]
    sealed class ArchetypeDebugViewInspector : UnityEditor.Editor
    {
        SerializedProperty entityListProp;

        private void OnEnable()
        {
            entityListProp = serializedObject.FindProperty("EntityViewList");
        }
        public override void OnInspectorGUI()
        {
            var debugView = (ArchetypeDebugView)target;
            if (debugView.World == null)
                return;
            if (!debugView.gameObject.activeSelf)
                return;

            var world = debugView.World;
            var archetype = debugView.Archetype;
            var system = debugView.System;

            GUI.enabled = false;
            //querydesc
            var segment = archetype.CompIds;
            EditorGUILayout.LabelField("Comps");
            for (int i = 0; i < segment.Count; i++)
                EditorGUILayout.HelpBox($"{segment.Array[i]}:{world.GetComponentType(segment.Array[i]).Name}", MessageType.None);

            var links = archetype.Nexts;
            // Debug.Log($"Nexts: {string.Join<EcsArchetype>(",", links)}");
            EditorGUILayout.LabelField("NextArchetypes");
            for (int i = 0; i < links.Length; i++)
            {
                var linkArchetype = links[i];
                if (linkArchetype == null) continue;
                var archetypeView = system.GetArchetypeView(linkArchetype.Id);
                // Debug.Log($"Nexts: {i}:{world.GetComponentType((byte)i).Name}, {archetypeView}");
                EditorGUILayout.ObjectField($"{i}:{world.GetComponentType((byte)i).Name}", archetypeView, typeof(ArchetypeDebugView), true);
            }
            links = archetype.Pres;
            EditorGUILayout.LabelField("PreArchetypes");
            for (int i = 0; i < links.Length; i++)
            {
                var linkArchetype = links[i];
                if (linkArchetype == null) continue;
                var archetypeView = system.GetArchetypeView(linkArchetype.Id);
                // Debug.Log($"Pres: {i}:{world.GetComponentType(i).Name}, {archetypeView}");
                EditorGUILayout.ObjectField($"{i}:{world.GetComponentType((byte)i).Name}", archetypeView, typeof(ArchetypeDebugView), true);
            }

            EditorGUILayout.PropertyField(entityListProp, true);
            GUI.enabled = true;
        }
    }
}
