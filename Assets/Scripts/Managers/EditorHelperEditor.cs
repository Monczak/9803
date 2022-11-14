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

            if (GUILayout.Button("Force Reserialize Assets"))
            {
                AssetDatabase.ForceReserializeAssets();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
    #endif
}