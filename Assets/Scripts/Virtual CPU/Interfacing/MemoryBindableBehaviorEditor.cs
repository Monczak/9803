using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;

namespace NineEightOhThree.VirtualCPU.Interfacing
{
    [CustomEditor(typeof(MemoryBindableBehavior), true)]
    [CanEditMultipleObjects]
    public class MemoryBindableBehaviorEditor : Editor
    {
        private MemoryBindableBehavior behavior;

        private List<SerializedProperty> serializedBindableProps;

        private void OnEnable()
        {
            behavior = target as MemoryBindableBehavior;
            serializedBindableProps = new();

            behavior.testFields = new();

            foreach (var f in behavior.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(f => f.FieldType == typeof(Bindable)))
            {
                SerializedProperty bindableProp = serializedObject.FindProperty(f.Name);
                serializedBindableProps.Add(bindableProp);

                Bindable bindable = f.GetValue(behavior) as Bindable;
                if (bindable == null)
                    bindable = CreateInstance<Bindable>();

                bindableProp.objectReferenceValue = bindable;

                bindableProp.serializedObject.ApplyModifiedProperties();

                behavior.testFields.Add(f);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            foreach (var prop in serializedBindableProps)
            {
                prop.serializedObject.Update();
                Bindable bindable = prop.objectReferenceValue as Bindable;
                bindable.SetValue((byte)EditorGUILayout.DelayedIntField(bindable.GetValue<byte>()));
                prop.serializedObject.ApplyModifiedProperties();
            }

            // Debug.Log(((Bindable)serializedBindableProps[0].objectReferenceValue).GetValue<byte>());

            //if (behavior == null)
            //    return;

            //if (behavior.bindableValues == null)
            //    Debug.LogError("Bindable values are null!");

            //behavior.bindableValues ??= new();
            //behavior.addresses ??= new();

            //EditorGUILayout.Separator();
            //EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.LabelField("Bindables", EditorStyles.boldLabel);
            //EditorGUILayout.EndHorizontal();

            //serializedObject.Update();
            //foreach (SerializedProperty prop in serializedBindables)
            //{
            //    EditorGUILayout.PropertyField(prop);
            //}
            //serializedObject.ApplyModifiedProperties();

            //foreach (var f in behavior.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(f => f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition() == typeof(Bindable<>)))
            //{
            //    Bindable bindable = f.GetValue(behavior) as Bindable;
            //    Type bindableType = f.FieldType.GetGenericArguments()[0];

            //    bindable ??= (Bindable)Activator.CreateInstance(f.FieldType);
            //    f.SetValue(behavior, bindable);

            //    if (!behavior.bindableValues.ContainsKey(bindable))
            //        behavior.bindableValues[bindable] = "";
            //    if (!behavior.addresses.ContainsKey(f))
            //        behavior.addresses[f] = 0;

            //    EditorGUILayout.BeginHorizontal();
            //    string value = EditorGUILayout.DelayedTextField(Beautify(f.Name), behavior.bindableValues[bindable]);
            //    string addressStr = EditorGUILayout.TextField("Address", behavior.addresses[f].ToString("X4"));
            //    EditorGUILayout.EndHorizontal();

            //    if (value != behavior.bindableValues[bindable])
            //    {
            //        bindable.SetValueFromString(value, bindableType);
            //    }

            //    behavior.bindableValues[bindable] = value;
            //    if (ushort.TryParse(addressStr, out ushort address))
            //    {
            //        behavior.addresses[f] = address;
            //    }
            //}
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
    }
}
