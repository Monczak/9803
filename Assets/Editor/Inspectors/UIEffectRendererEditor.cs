using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<EffectAnimationList> animations;
        
        private Dictionary<string, int> effectIndexes;
        private Dictionary<string, Dictionary<string, int>> propertyIndexes;

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

        private void UpdateEffectIndex(string effectName, int index)
        {
            effectIndexes ??= new Dictionary<string, int>();
            effectIndexes[effectName] = index;
        }

        private int GetEffectIndex(string effectName)
        {
            return effectIndexes[effectName];
        }
        
        private void UpdatePropertyIndex(string effectName, string propertyName, int index)
        {
            propertyIndexes ??= new Dictionary<string, Dictionary<string, int>>();
            if (!propertyIndexes.ContainsKey(effectName))
                propertyIndexes.Add(effectName, new Dictionary<string, int>());
            propertyIndexes[effectName][propertyName] = index;
        }

        private bool HasProperty(string effectName, string propertyName)
        {
            return propertyIndexes[effectName].ContainsKey(propertyName);
        }

        private int GetPropertyIndex(string effectName, string propertyName)
        {
            return propertyIndexes[effectName][propertyName];
        }

        private void OnEnable()
        {
            theRenderer = target as UIEffectRenderer;
            if (theRenderer is null) return;

            effects = theRenderer.effects;
            animations = theRenderer.animations;

            UpdateIndexes();
        }

        private void UpdateIndexes()
        {
            for (int i = 0; i < effects.Count; i++)
            {
                var effect = effects[i];
                UpdateEffectIndex(effect.Name, i);
                for (int j = 0; j < effect.propertyList.Count; j++)
                {
                    UpdatePropertyIndex(effect.Name, effect.propertyList[j].Name, j);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();
            
            serializedObject.Update();
            
            PropertyField("input");
            PropertyField("animationTime");
            
            DrawEffectList();
            
            UpdateIndexes();
            
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
            list ??= new List<T>();
            
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
                    (effect, i) =>
                    {
                        UpdateEffectIndex(effect.Name, i);

                        if (BeginFoldout(effect, effect.HasMaterial ? effect.Name : "(no material)"))
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
                    UpdatePropertyIndex(effect.Name, effect.propertyList[i].Name, i);
                    
                    string propertyPath = $"{effectPath}.propertyList.Array.data[{i}]";
                    PropertyField(prop: $"{propertyPath}.Value", isCSharpProperty: true, label: effect.propertyList[i].NiceName);
                }
            }
        }

        private void DrawAnimationList()
        {
            if (BeginListAddFoldout("AnimationListFoldout", "Animations", animations))
            {
                using var scope = new IndentedScope();

                using var reorderableListScope = new ReorderableListScope<EffectAnimationList>(animations,
                    DrawPropertyAnimationList
                );
            }
        }

        private void DrawPropertyAnimationList(EffectAnimationList animationList, int animationListIndex)
        {
            if (BeginFoldout(animationList, animationList.Name))
            {
                using var scope = new IndentedScope();
                PropertyField($"animations.Array.data[{animationListIndex}].Name", isCSharpProperty: true);
                
                if (BeginListAddFoldout(animationList.PropertyAnimations, "Property Animations", animationList.PropertyAnimations))
                {
                    using var scope2 = new IndentedScope();

                    using var reorderableListScope = new ReorderableListScope<EffectAnimation>(
                        animationList.PropertyAnimations,
                        (anim, animIndex) => DrawPropertyAnimationEditor(anim, animIndex, animationListIndex));
                }
            }
            
            
        }

        private void DrawPropertyAnimationEditor(EffectAnimation animation, int animationIndex, int animationListIndex) // FIXME
        {
            using (new GUILayout.HorizontalScope())
            {
                var effectNames = effects.Where(e => e.HasMaterial).Select(e => e.Name).ToArray();
                int index = animation.Effect is null || !animation.Effect.HasMaterial ? 0 : GetEffectIndex(animation.Effect.Name);
                index = EditorGUILayout.Popup(index, effectNames);
                animation.Effect = effects[index];

                var propertyNames = animation.Effect.Properties.Values.Select(p => p.NiceName).ToArray();
                index = string.IsNullOrEmpty(animation.PropertyName) || !animation.Effect.HasProperty(animation.PropertyName) ? 0 : GetPropertyIndex(animation.Effect.Name, animation.PropertyName);
                index = EditorGUILayout.Popup(index, propertyNames);
                animation.PropertyName = animation.Effect.propertyList[index].Name;

                PropertyField($"animations.Array.data[{animationListIndex}].<PropertyAnimations>k__BackingField.Array.data[{animationIndex}].animationCurve", label: "");
            }
        }
    }
}