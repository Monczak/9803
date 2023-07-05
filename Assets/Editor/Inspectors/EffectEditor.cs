using System;
using NineEightOhThree.Rendering.Effects;
using UnityEditor;

namespace NineEightOhThree.Editor.Inspectors
{
    [CustomEditor(typeof(Effect)), CanEditMultipleObjects]
    public class EffectEditor : UnityEditor.Editor
    {
        private Effect effect;

        private void OnEnable()
        {
            effect = target as Effect;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawEffectPropertyList();
            
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawEffectPropertyList()
        {
            foreach (EffectProperty property in effect.Properties.Values)
            {
                property.Value = EditorGUILayout.FloatField(property.NiceName, property.Value);
            }
        }
    }
}