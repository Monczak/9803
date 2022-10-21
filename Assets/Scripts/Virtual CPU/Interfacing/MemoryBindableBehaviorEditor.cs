using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU.Interfacing
{
    [CustomEditor(typeof(MemoryBindableBehavior), true)]
    [CanEditMultipleObjects]
    public class MemoryBindableBehaviorEditor : Editor
    {
        private MemoryBindableBehavior behavior;

        private void OnEnable()
        {
            behavior = target as MemoryBindableBehavior;

            if (behavior != null && behavior.bindables == null)
            {
                RefreshBindables();
            }

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Bindables", EditorStyles.boldLabel);

            foreach (var bindable in behavior.bindables)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel($"{Beautify(bindable.fieldName)} ({bindable.type})");

                string valueInput = EditorGUILayout.DelayedTextField(bindable.value?.ToString());
                bindable.SetValueFromString(valueInput);

                bindable.enabled = EditorGUILayout.Toggle(bindable.enabled);

                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < bindable.Bytes; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    string addressInput =
                        EditorGUILayout.DelayedTextField($"Byte {i}", bindable.addresses[i].ToString("X4"));
                    if (ushort.TryParse(addressInput, System.Globalization.NumberStyles.HexNumber, null,
                            out ushort address))
                    {
                        bindable.addresses[i] = address;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Separator();
            }

            if (GUILayout.Button("Refresh Bindables"))
            {
                RefreshBindables();
            }
        }

        private string Beautify(string fieldName)
        {
            string[] strings = Regex.Split(fieldName, @"(?<!^)(?=[A-Z])");
            StringBuilder builder = new();
            foreach (string str in strings)
                builder.Append(str[0].ToString().ToUpper()).Append(str.AsSpan(1)).Append(" ");
            builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }

        private void RefreshBindables()
        {
            behavior.bindables = new List<Bindable>();

            foreach (var f in behavior.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(f => f.FieldType == typeof(Bindable)))
            {
                Bindable bindable = CreateInstance<Bindable>();
                bindable.fieldName = f.Name;

                if (f.IsDefined(typeof(BindableTypeAttribute)))
                {
                    bindable.type = ((BindableTypeAttribute)f.GetCustomAttribute(typeof(BindableTypeAttribute))).type;
                }
                else
                {
                    bindable.type = BindableType.Byte;
                }

                bindable.addresses = new ushort[bindable.Bytes];
                f.SetValue(behavior, bindable);
                behavior.bindables.Add(bindable);
            }
        }
    }
}
