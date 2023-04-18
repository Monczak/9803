using System;
using System.Collections.Generic;
using NineEightOhThree.Rendering;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace NineEightOhThree.Editor.Inspectors
{
    [CustomEditor(typeof(UIEffectRenderer))]
    public class UIEffectRendererEditor : UnityEditor.Editor
    {
        private UIEffectRenderer theRenderer;

        private List<Effect> effects;

        private Dictionary<object, bool> foldOuts;
        private Dictionary<string, SerializedProperty> properties;

        private SerializedProperty FindProperty(string prop, bool isCsharpProperty = false)
        {
            properties ??= new Dictionary<string, SerializedProperty>();

            int lastIndex = prop.LastIndexOf(".", StringComparison.Ordinal);
            string first, last;
            if (lastIndex == -1)
                (first, last) = ("", prop);
            else
                (first, last) = (prop[..(lastIndex + 1)], prop[(lastIndex + 1)..]);
            string realProp = isCsharpProperty ? $"{first}<{last}>k__BackingField" : prop;
            
            if (!properties.ContainsKey(realProp))
                properties[realProp] = serializedObject.FindProperty(realProp);

            if (properties[realProp] is null)
                Debug.LogError($"{realProp} does not exist!");
            
            return properties[realProp];
        }

        private bool PropertyField(string prop, bool isCsharpProperty = false, string label = null)
        {
            if (label is null)
                return EditorGUILayout.PropertyField(FindProperty(prop, isCsharpProperty));
            return EditorGUILayout.PropertyField(FindProperty(prop, isCsharpProperty), new GUIContent(label));
        }

        private void SetFoldout(object obj, bool visible)
        {
            foldOuts ??= new Dictionary<object, bool>();
            foldOuts[obj] = visible;
        }

        private bool GetFoldout(object obj)
        {
            if (foldOuts is null) return false;
            if (!foldOuts.ContainsKey(obj)) return false;
            return foldOuts[obj];
        }

        private void OnEnable()
        {
            theRenderer = target as UIEffectRenderer;
            if (theRenderer is null) return;

            effects = theRenderer.effects;
        }

        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();
            
            serializedObject.Update();

            PropertyField("input");
            PropertyField("animationTime");

            DrawEffectList();

            bool reloadMaterials = GUILayout.Button("Reload Effects");
            if (reloadMaterials)
            {
                theRenderer.InitializeEffects(destructive: true);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private bool BeginFoldout(object obj, string name)
        {
            SetFoldout(obj, EditorGUILayout.Foldout(GetFoldout(obj), name));
            return GetFoldout(obj);
        }

        private void DrawEffectList()
        {
            if (BeginFoldout("EffectFoldout", "Effects"))
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Space(10);
                    using (new GUILayout.VerticalScope())
                    {
                        for (int i = 0; i < effects.Count; i++)
                        {
                            DrawEffectEditor(i);
                        }
                    }
                }
            }
        }

        private void DrawEffectEditor(int effectIndex)
        {
            var effect = effects[effectIndex];

            if (BeginFoldout(effect, effect.HasMaterial ? effect.Material.name : "(no material)"))
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Space(10);
                    using (new GUILayout.VerticalScope())
                    {
                        string effectPath = $"effects.Array.data[{effectIndex}]";
                        
                        PropertyField($"{effectPath}.sourceMaterial");
                        PropertyField($"{effectPath}.enabled");

                        if (BeginFoldout(effect.propertyList, "Properties"))
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.Space(10);
                                using (new GUILayout.VerticalScope())
                                {
                                    for (int i = 0; i < effect.propertyList.Count; i++)
                                    {
                                        string propertyPath = $"{effectPath}.propertyList.Array.data[{i}]";
                                        
                                        PropertyField(prop: $"{propertyPath}.Value", isCsharpProperty: true, label: effect.propertyList[i].Name);
                                        
                                    }
                                    PropertyField($"{effectPath}.test");
                                }
                            }
                        }
                        // PropertyField($"{effectPath}.propertyList");
                    }
                }
            }

            
        }
    }
}