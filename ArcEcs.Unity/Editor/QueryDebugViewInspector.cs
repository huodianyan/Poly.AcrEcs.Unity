using UnityEditor;
using UnityEngine;

namespace Poly.ArcEcs.Unity.Editor
{
    [CustomEditor(typeof(QueryDebugView))]
    sealed class QueryDebugViewInspector : UnityEditor.Editor
    {
        SerializedProperty archetypeListProp;

        private void OnEnable()
        {
            // Setup the SerializedProperties.
            archetypeListProp = serializedObject.FindProperty("ArchetypeViewList");
        }
        public override void OnInspectorGUI()
        {
            var debugView = (QueryDebugView)target;
            if (debugView.World == null)
                return;
            if (!debugView.gameObject.activeSelf)
                return;

            var world = debugView.World;
            var query = debugView.Query;
            var queryDesc = query.QueryDesc;

            GUI.enabled = false;
            //querydesc
            var segment = queryDesc.All;
            EditorGUILayout.LabelField("AllComps");
            for (int i = 0; i < segment.Count; i++)
                EditorGUILayout.HelpBox($"{segment.Array[i]}:{world.GetComponentType(segment.Array[i]).Name}", MessageType.None);
            segment = queryDesc.Any;
            EditorGUILayout.LabelField("AnyComps");
            for (int i = 0; i < segment.Count; i++)
                EditorGUILayout.HelpBox($"{segment.Array[i]}:{world.GetComponentType(segment.Array[i]).Name}", MessageType.None);
            segment = queryDesc.None;
            EditorGUILayout.LabelField("NoneComps");
            for (int i = 0; i < segment.Count; i++)
                EditorGUILayout.HelpBox($"{segment.Array[i]}:{world.GetComponentType(segment.Array[i]).Name}", MessageType.None);

            // EditorGUILayout.TextField("AllComps", world.GetCompnentsName(segment.Array, segment.Count));
            // segment = queryDesc.Any;
            // EditorGUILayout.TextField("AnyComps", world.GetCompnentsName(segment.Array, segment.Count));
            // segment = queryDesc.None;
            // EditorGUILayout.TextField("NoneComps", world.GetCompnentsName(segment.Array, segment.Count));

            EditorGUILayout.PropertyField(archetypeListProp, true);
            GUI.enabled = true;
        }
    }
}
