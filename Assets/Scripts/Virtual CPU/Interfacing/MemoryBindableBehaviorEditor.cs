using System;
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

            if (behavior.bindables == null)
            {
                RefreshBindables();
            }

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Bindables", EditorStyles.boldLabel);

            for (int i = 0; i < behavior.bindables.Count; i++)
            {
                Bindable bindable = behavior.bindables[i];

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel($"{Beautify(bindable.fieldName)} ({bindable.type})");

                string valueInput = EditorGUILayout.DelayedTextField(bindable.value?.ToString());
                bindable.SetValueFromString(valueInput);

                string addressInput = EditorGUILayout.DelayedTextField(bindable.address.ToString("X4"));
                if (ushort.TryParse(addressInput, System.Globalization.NumberStyles.HexNumber, null, out ushort address))
                {
                    bindable.address = address;
                }

                bindable.enabled = EditorGUILayout.Toggle(bindable.enabled);

                EditorGUILayout.EndHorizontal();
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
            behavior.bindables = new();

            foreach (var f in behavior.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(f => f.FieldType == typeof(Bindable)))
            {
                Bindable bindable = CreateInstance<Bindable>();
                bindable.fieldName = f.Name;

                if (f.IsDefined(typeof(BindableTypeAttribute)))
                {
                    bindable.type = (f.GetCustomAttribute(typeof(BindableTypeAttribute)) as BindableTypeAttribute).type;
                }
                else
                {
                    bindable.type = BindableType.Byte;
                }

                f.SetValue(behavior, bindable);
                behavior.bindables.Add(bindable);
            }
        }
    }
}
