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
    [CustomEditor(typeof(MemoryInterface))]
    public class MemoryInterfaceEditor : Editor
    {
        private MemoryInterface memoryInterface;

        private Dictionary<FieldInfo, ushort> addresses;
        private bool showAllFields = false;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Show All Fields");
            showAllFields = EditorGUILayout.Toggle(showAllFields);
            EditorGUILayout.EndHorizontal();

            memoryInterface = target as MemoryInterface;
            if (memoryInterface == null)
                return;

            addresses ??= new();

            Component component = memoryInterface.target;

            HashSet<FieldInfo> referencedFields = new();
            EditorGUILayout.Separator();
            foreach (var f in component.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(f => f.IsDefined(typeof(MemoryBindableAttribute), false) || showAllFields))
            {
                if (!addresses.ContainsKey(f))
                    addresses[f] = 0;

                referencedFields.Add(f);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"{Beautify(f.Name)} Address{(f.FieldType != typeof(byte) ? " (not byte)" : "")}");

                string addressStr = EditorGUILayout.TextField(addresses[f].ToString("X4"));
                if (!ushort.TryParse(addressStr, System.Globalization.NumberStyles.HexNumber, null, out ushort address))
                {
                    // Invalid address
                }
                else
                {
                    addresses[f] = address;
                }
                EditorGUILayout.EndHorizontal();
            }

            // Just so the dict doesn't clog up memory
            foreach (FieldInfo f in addresses.Keys.ToList())
                if (!referencedFields.Contains(f))
                    addresses.Remove(f);

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
