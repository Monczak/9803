using System.Collections.Generic;
using System.Linq;
using NineEightOhThree.Dialogues;
using SamSharp;
using SamSharp.Parser;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace NineEightOhThree.Editor.Inspectors
{
    [CustomEditor(typeof(Dialogue)), CanEditMultipleObjects]
    public class DialogueEditor : UnityEditor.Editor
    {
        private Dialogue dialogue;

        private Dictionary<DialogueLine, bool> foldOuts;

        private Sam sam;
        
        private Dictionary<DialogueLine, Parser.PhonemeData[]> phonemeData;
        private Dictionary<DialogueLine, bool> showPhonemes;
        private Dictionary<DialogueLine, Dictionary<int, bool>> keyframeEnabled;

        private void OnEnable()
        {
            dialogue = target as Dialogue;
            foldOuts ??= new Dictionary<DialogueLine, bool>();
            phonemeData ??= new Dictionary<DialogueLine, Parser.PhonemeData[]>();
            showPhonemes = new Dictionary<DialogueLine, bool>();
            keyframeEnabled ??= new Dictionary<DialogueLine, Dictionary<int, bool>>();

            if (dialogue is not null)
            {
                if (foldOuts.Count == 0)
                {
                    foreach (DialogueLine line in dialogue.Lines)
                    {
                        foldOuts[line] = false;
                        phonemeData[line] = null;
                        showPhonemes[line] = false;
                        keyframeEnabled[line] = new Dictionary<int, bool>();
                    }
                }
                
            }

            sam = new Sam();
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
                    line.Options.PitchModifier = EditorGUILayout.CurveField("Pitch", line.Options.PitchModifier, Color.red, new Rect(0, 0, 1, 2));
                    line.Options.SpeedModifier ??= new AnimationCurve();
                    line.Options.SpeedModifier = EditorGUILayout.CurveField("Speed", line.Options.SpeedModifier, Color.cyan, new Rect(0, 0, 1, 2));
                    
                    EditorGUILayout.Separator();

                    if (GUILayout.Button("Get Phoneme Data"))
                    {
                        sam.Options = line.Options;
                        sam.GetPhonemeDataAsync(line.Text).ContinueWith(t =>
                        {
                            phonemeData[line] = t.Result;
                            Repaint();
                        });
                    }

                    showPhonemes[line] = EditorGUILayout.Foldout(showPhonemes[line], "Phoneme Data");
                    if (showPhonemes[line])
                    {
                        if (phonemeData[line] is not null)
                        {
                            DrawPhonemeTable(line);
                        }
                        else
                        {
                            EditorGUILayout.LabelField("No phoneme data exists yet.");
                        }
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }

        private void DrawPhonemeTable(DialogueLine line)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            EditorGUILayout.Space();

            float w = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 40;
            EditorGUILayout.LabelField("Phoneme", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Length", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Keyframe", EditorStyles.boldLabel);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < phonemeData[line].Length; i++)
            {
                Parser.PhonemeData data = phonemeData[line][i];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();

                EditorGUIUtility.labelWidth = 40;
                EditorGUILayout.LabelField(data.PhonemeName);
                EditorGUILayout.LabelField(data.Length.ToString());

                if (data.WordStart && !keyframeEnabled[line].ContainsKey(i))
                    keyframeEnabled[line][i] = true;
                
                keyframeEnabled[line].TryGetValue(i, out bool enabled);
                keyframeEnabled[line][i] = EditorGUILayout.Toggle(enabled);

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUIUtility.labelWidth = w;
        }
    }
}