using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Poly.ArcEcs.Unity.Editor
{
    [CustomEditor(typeof(GOEntity))]
    sealed class GOEntityInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var debugView = (GOEntity)target;
            if (debugView.World == null)
                return;
            var world = debugView.World;
            var entity = debugView.Entity;

            GUI.enabled = false;
            EditorGUILayout.TextField("World", world.Id);
            EditorGUILayout.IntField("EntityId", entity.Index);
            EditorGUILayout.IntField("EntityVersion", entity.Version);
            GUI.enabled = true;
        }
    }


}
