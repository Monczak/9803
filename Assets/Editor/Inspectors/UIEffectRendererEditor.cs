using System;
using NineEightOhThree.Rendering;
using UnityEditor;
using UnityEngine;

namespace NineEightOhThree.Editor.Inspectors
{
    [CustomEditor(typeof(UIEffectRenderer)), CanEditMultipleObjects]
    public class UIEffectRendererEditor : UnityEditor.Editor
    {
        private UIEffectRenderer theRenderer;

        private void OnEnable()
        {
            theRenderer = target as UIEffectRenderer;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            bool reloadMaterials = GUILayout.Button("Reload Effects");
            if (reloadMaterials)
            {
                theRenderer.InitializeEffects(destructive: true);
            }
        }
    }
}