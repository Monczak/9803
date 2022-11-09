using NineEightOhThree.Inventories;
using UnityEditor;
using UnityEngine;

namespace NineEightOhThree.Managers
{
    #if UNITY_EDITOR
    [CustomEditor(typeof(EditorHelper))]
    public class EditorHelperEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reload Items"))
            {
                ItemRegistry.RegisterItems();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
    #endif
}