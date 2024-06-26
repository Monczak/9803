﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using NineEightOhThree.Utilities;
using UnityEditor;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU.Interfacing
{
    #if UNITY_EDITOR
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
                if (bindable.type == BindableType.Object)
                {
                    bindable.SetValueNullIfSerializedNull();

                    Type type = Type.GetType(bindable.objectTypeName);
                    if (type is not null)
                    {
                        bindable.CreateNewValue();
                    } 
                    bindable.DeserializeIfHasData();
                }

                bindable.parentObjectName = behavior.gameObject.name;
                bindable.parentClassName = behavior.GetType().Name;
                
                EditorGUILayout.BeginHorizontal();
                bindable.enabled = EditorGUILayout.Toggle(StringUtils.Beautify(bindable.fieldName), bindable.enabled);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                string label = @$"({(bindable.type == BindableType.Object 
                    ? StringUtils.Beautify(bindable.objectTypeName[(bindable.objectTypeName.LastIndexOf(".", StringComparison.InvariantCulture)+1)..]) 
                    : bindable.type.ToString())})";

                string valueInput = EditorGUILayout.DelayedTextField(label, bindable.type == BindableType.Object
                    ? ((ISerializableBindableObject)bindable.value)?.Serialize()
                    : bindable.value?.ToString());
                if (valueInput != "")
                {
                    bindable.SetValueFromString(valueInput);
                    if (bindable.type == BindableType.Object) bindable.ForceSerialize();
                }

                EditorGUILayout.EndHorizontal();

                if (bindable.Bytes == 0)
                {
                    EditorGUILayout.LabelField("No addresses (0 bytes)");
                }
                else
                {
                    for (int i = 0; i < bindable.AddressCount; i++)
                    {
                        EditorGUILayout.BeginHorizontal();

                        var addressNames = bindable.type == BindableType.Object ? null : Bindable.Handlers[bindable.type].AddressNames;
                    
                        string addressInput =
                            EditorGUILayout.DelayedTextField(addressNames is not null ? addressNames[i] : (bindable.AddressCount == 1 ? "Address" : $"Byte {i}"), bindable.addresses[i].ToString("X4"));
                        if (ushort.TryParse(addressInput, System.Globalization.NumberStyles.HexNumber, null,
                                out ushort address))
                        {
                            bindable.addresses[i] = address;
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }
                
                EditorGUILayout.Separator();
            }
                        
            if (GUILayout.Button("Refresh Bindables"))
            {
                RefreshBindables();
            }
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
                    BindableTypeAttribute attrib =
                        (BindableTypeAttribute)f.GetCustomAttribute(typeof(BindableTypeAttribute));
                    bindable.type = attrib.type;
                    
                    if (attrib.type == BindableType.Object)
                    {
                        bindable.objectTypeName = attrib.objectType.FullName;
                    }
                }
                else
                {
                    bindable.type = BindableType.Byte;
                }

                if (bindable.type == BindableType.Object)
                {
                    if (bindable.objectTypeName is null)
                    {
                        throw new Exception("Bindable object type name is null");
                    }
                    
                    Type type = Type.GetType(bindable.objectTypeName);
                    if (type is null)
                    {
                        throw new Exception("Bindable type does not exist");
                    }

                    if (type.IsSubclassOf(typeof(ScriptableObject)))
                        bindable.value = CreateInstance(type);
                    else
                        bindable.value = Activator.CreateInstance(type);
                }

                bindable.addresses = new ushort[bindable.AddressCount];
                
                f.SetValue(behavior, bindable);
                behavior.bindables.Add(bindable);
            }
        }
    }
    #endif
}
