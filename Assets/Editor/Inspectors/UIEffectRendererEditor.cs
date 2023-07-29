using System;
using NineEightOhThree.Rendering.Effects;
using UnityEditor;
using UnityEngine;

namespace NineEightOhThree.Editor.Inspectors
{
    [CustomEditor(typeof(UIEffectRenderer))]
    public class UIEffectRendererEditor : UnityEditor.Editor
    {
        private UIEffectRenderer theRenderer;

        private void OnEnable()
        {
            theRenderer = target as UIEffectRenderer;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }

        
    }
}