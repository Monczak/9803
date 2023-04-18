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

            /*if (properties[realProp] is null)
                Debug.LogError($"{realProp} does not exist!");*/
            
            return properties[realProp];
        }

        private bool PropertyField(string prop, bool isCSharpProperty = false, string label = null)
        {
            if (label is null)
                return EditorGUILayout.PropertyField(FindProperty(prop, isCSharpProperty));
            return EditorGUILayout.PropertyField(FindProperty(prop, isCSharpProperty), new GUIContent(label));
        }

        private void SetFoldout(object key, bool visible)
        {
            foldOuts ??= new Dictionary<object, bool>();

            if (key is null) return;
            foldOuts[key] = visible;
        }

        private bool GetFoldout(object key)
        {
            if (key is null || foldOuts is null) return false;
            if (!foldOuts.ContainsKey(key)) return false;
            return foldOuts[key];
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

            DrawAnimationList();

            bool reloadMaterials = GUILayout.Button("Reload Effects");
            if (reloadMaterials)
            {
                theRenderer.InitializeEffects(destructive: true);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private bool BeginFoldout(object key, string name)
        {
            SetFoldout(key, EditorGUILayout.Foldout(GetFoldout(key), name, EditorStyles.foldoutHeader));
            return GetFoldout(key);
        }

        private bool BeginListAddFoldout<T>(object key, string name, ICollection<T> list)
        {
            using var horizontalScope = new GUILayout.HorizontalScope();
                
            bool open;
            using (new GUILayout.VerticalScope())
            {
                open = BeginFoldout(key, name);
            }

            if (GUILayout.Button("+", new GUIStyle(EditorStyles.miniButton) { fixedWidth = 20 }))
            {
                T obj = (T)Activator.CreateInstance(typeof(T));
                list.Add(obj);
            }

            return open;
        }

        private void DrawEffectList()
        {
            if (BeginListAddFoldout("EffectFoldout", "Effects", effects))
            {
                using var scope = new IndentedScope();

                using var reorderableListScope = new ReorderableListScope<Effect>(effects,
                    (element, i) =>
                    {
                        if (BeginFoldout(element, element.HasMaterial ? element.Material.name : "(no material)"))
                        {
                            using var scope2 = new IndentedScope();
                            DrawEffectEditor(i);
                        }
                    }
                );
            }
        }

        private void DrawEffectEditor(int effectIndex)
        {
            var effect = effects[effectIndex];

            string effectPath = $"effects.Array.data[{effectIndex}]";
                        
            PropertyField($"{effectPath}.sourceMaterial");
            PropertyField($"{effectPath}.enabled");

            if (BeginFoldout(effect.propertyList, "Properties"))
            {
                using var scope2 = new IndentedScope();
                    
                for (int i = 0; i < effect.propertyList.Count; i++)
                {
                    string propertyPath = $"{effectPath}.propertyList.Array.data[{i}]";
                    PropertyField(prop: $"{propertyPath}.Value", isCSharpProperty: true, label: effect.propertyList[i].NiceName);
                }
            }
        }

        private void DrawAnimationList()
        {
            // TODO
        }
        
        private void DrawAnimationEditor()
        {
            // TODO
        }
    }
}