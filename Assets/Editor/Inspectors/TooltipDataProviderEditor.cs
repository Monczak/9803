using System;
using System.Collections.Generic;
using System.Linq;
using NineEightOhThree.UI.BindableFormatters;
using NineEightOhThree.UI.Tooltips;
using NineEightOhThree.VirtualCPU.Interfacing;
using UnityEditor;
using UnityEngine;

namespace NineEightOhThree.Editor.Inspectors
{
    [CustomEditor(typeof(TooltipDataProvider)), CanEditMultipleObjects]
    public class TooltipDataProviderEditor : UnityEditor.Editor
    {
        private TooltipDataProvider dataProvider;

        private void OnEnable()
        {
            dataProvider = target as TooltipDataProvider;
            if (dataProvider)
            {
                dataProvider.FindBindables();
            } 
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            base.OnInspectorGUI();

            foreach (var (bindableKey, tooltipData) in dataProvider.dataRules)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel(bindableKey);
                    tooltipData.enabled = EditorGUILayout.Toggle(tooltipData.enabled);
                    tooltipData.name = EditorGUILayout.TextField(tooltipData.name);
                    tooltipData.formatterIndex = EditorGUILayout.Popup(tooltipData.formatterIndex, BindableFormatterRegistry.BindableTypes);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}