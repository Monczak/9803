using System;
using System.Collections.Generic;
using System.Linq;
using NineEightOhThree.Rendering.Effects;
using UnityEditor;
using UnityEngine;

namespace NineEightOhThree.Editor.Inspectors
{
    [CustomEditor(typeof(EffectAnimation))]
    public class EffectAnimationEditor : UnityEditor.Editor
    {
        private EffectAnimation animation;

        private void OnEnable()
        {
            animation = target as EffectAnimation;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            using (new EditorGUILayout.HorizontalScope())
            {
                bool enabled = animation.usedEffects.Count > 0;
                GUI.enabled = enabled;
                if (GUILayout.Button("New Curve"))
                {
                    animation.AnimatedProperties.Add(new AnimatedProperty(animation.usedEffects[0], animation.usedEffects[0].Properties.Values.First().Name));
                }
                GUI.enabled = true;

                if (!enabled)
                    animation.AnimatedProperties.Clear();
            }
            
            using (new ReorderableListScope<AnimatedProperty>(
                       animation.AnimatedProperties,
                       DrawAnimatedProperty))
            {
                
            }
        }

        private void DrawAnimatedProperty(AnimatedProperty property, int index)
        {
            using var scope = new EditorGUILayout.HorizontalScope();

            DrawEffectDropdown(property);
            DrawPropertyDropdown(property);
            DrawPropertyCurve(property);
        }
        
        private void DrawEffectDropdown(AnimatedProperty property)
        {
            int selected = animation.usedEffects.IndexOf(property.Effect);
            if (selected == -1) selected = 0;
            selected = EditorGUILayout.Popup(selected, animation.usedEffects.Select(e => e.Name).ToArray());
            property.Effect = animation.usedEffects[selected];
        }

        private void DrawPropertyDropdown(AnimatedProperty property)
        {
            var properties = property.Effect.Properties;
            int selected = properties.Keys.ToList().IndexOf(property.PropertyName);
            if (selected == -1)
            {
                selected = 0;
                property.PropertyName = properties.Values.First().Name;
            }

            selected = EditorGUILayout.Popup(selected, properties.Values.Select(p => p.NiceName).ToArray());

            property.PropertyName = properties.Values.ToList()[selected].Name;
        }
        
        private void DrawPropertyCurve(AnimatedProperty property)
        {
            property.AnimationCurve = EditorGUILayout.CurveField(property.AnimationCurve);
        }
    }
}
