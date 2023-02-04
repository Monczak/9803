using System;
using System.Collections.Generic;
using NineEightOhThree.Dialogues;
using UnityEditor;
using UnityEngine;

namespace NineEightOhThree.Editor.Inspectors
{
    [CustomEditor(typeof(Dialogue)), CanEditMultipleObjects]
    public class DialogueEditor : UnityEditor.Editor
    {
        private Dialogue dialogue;

        private Dictionary<DialogueLine, bool> foldOuts;

        private void OnEnable()
        {
            dialogue = target as Dialogue;
            foldOuts ??= new Dictionary<DialogueLine, bool>();

            if (dialogue is not null)
            {
                if (foldOuts.Count == 0)
                {
                    foreach (DialogueLine line in dialogue.Lines)
                    {
                        foldOuts[line] = false;
                    }
                }
                
            }
        }

        public override void OnInspectorGUI()
        {
            foreach (DialogueLine line in dialogue.Lines)
            {
                foldOuts[line] = EditorGUILayout.BeginFoldoutHeaderGroup(foldOuts[line], line.Text);
                if (foldOuts[line])
                {
                    EditorGUILayout.PrefixLabel("Text");
                    line.Text = EditorGUILayout.TextArea(line.Text);
                    
                    EditorGUILayout.Separator();
                    
                    EditorGUILayout.LabelField("SAM Parameters", EditorStyles.boldLabel);
                    line.Options.Pitch = (byte)EditorGUILayout.IntSlider("Pitch", line.Options.Pitch, 0, 255);
                    line.Options.Mouth = (byte)EditorGUILayout.IntSlider("Mouth", line.Options.Mouth, 0, 255);
                    line.Options.Throat = (byte)EditorGUILayout.IntSlider("Throat", line.Options.Throat, 0, 255);
                    line.Options.Speed = (byte)EditorGUILayout.IntSlider("Speed", line.Options.Speed, 1, 255);
                    line.Options.SingMode = EditorGUILayout.Toggle("Sing Mode", line.Options.SingMode);
                    
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField("Modifiers", EditorStyles.boldLabel);
                    line.Options.PitchModifier ??= new AnimationCurve();
                    line.Options.PitchModifier = EditorGUILayout.CurveField("Pitch", line.Options.PitchModifier);
                    line.Options.SpeedModifier ??= new AnimationCurve();
                    line.Options.SpeedModifier = EditorGUILayout.CurveField("Speed", line.Options.SpeedModifier);
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
    }
}