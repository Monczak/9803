using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NineEightOhThree.Dialogues;
using NineEightOhThree.Utilities;
using SamSharp;
using SamSharp.Parser;
using UnityEditor;
using UnityEngine;

namespace NineEightOhThree.Editor.Inspectors
{
    [CustomEditor(typeof(Dialogue)), CanEditMultipleObjects]
    public class DialogueEditor : UnityEditor.Editor
    {
        private Dialogue dialogue;

        private Dictionary<DialogueLine, bool> foldOuts;

        private Sam sam;
        
        private Dictionary<DialogueLine, PhonemeData[]> phonemeData;
        private Dictionary<DialogueLine, bool> showPhonemes;
        private Dictionary<DialogueLine, Dictionary<int, bool>> keyframeEnabled;
        private Dictionary<DialogueLine, List<int>> wordBoundaries;
        private Dictionary<DialogueLine, string> cachedText;

        private void OnEnable()
        {
            dialogue = target as Dialogue;
            foldOuts ??= new Dictionary<DialogueLine, bool>();
            phonemeData ??= new Dictionary<DialogueLine, PhonemeData[]>();
            showPhonemes = new Dictionary<DialogueLine, bool>();
            keyframeEnabled ??= new Dictionary<DialogueLine, Dictionary<int, bool>>();
            wordBoundaries ??= new Dictionary<DialogueLine, List<int>>();
            cachedText ??= new Dictionary<DialogueLine, string>();

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
                        wordBoundaries[line] = new List<int>();
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

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Get Phoneme Data"))
                    {
                        sam.GetPhonemeDataAsync(line.Text).ContinueWith(t =>
                        {
                            phonemeData[line] = t.Result;
                            showPhonemes[line] = true;
                            keyframeEnabled[line] = new Dictionary<int, bool>();
                            cachedText[line] = line.Text;
                            Repaint();
                        });
                    }

                    int keyframeCount = keyframeEnabled[line].Count(k => k.Value);
                    GUI.enabled = keyframeCount > 0;
                    if (GUILayout.Button($"Insert {keyframeCount} Keyframes"))
                    {
                        InsertKeyframes(line);
                    }
                    if (GUILayout.Button("Prepare Curves"))
                    {
                        PrepareCurves(line);
                    }
                    GUI.enabled = true;
                    EditorGUILayout.EndHorizontal();

                    showPhonemes[line] = EditorGUILayout.Foldout(showPhonemes[line], "Phoneme Data");
                    if (showPhonemes[line])
                    {
                        if (phonemeData[line] is not null)
                        {
                            DrawPhonemeTable(line);
                            SetKeyframes(line);
                            SetWordBoundaries(line);
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

        private void SetWordBoundaries(DialogueLine line)
        {
            line.WordBoundaryKeyframes = new List<float>();
            int totalLength = phonemeData[line].Sum(p => p.Length)!.Value;
            int cumulativeLength = 0;
            int boundaryIndex = 0;
            for (int i = 0; i < phonemeData[line].Length; i++)
            {
                if (boundaryIndex < wordBoundaries[line].Count && wordBoundaries[line][boundaryIndex] == i)
                {
                    line.WordBoundaryKeyframes.Add((float)cumulativeLength / totalLength);
                    boundaryIndex++;
                }
                cumulativeLength += phonemeData[line][i].Length!.Value;
            }
        }

        private void SetKeyframes(DialogueLine line)
        {
            line.Keyframes = new List<float>();
            int totalLength = phonemeData[line].Sum(p => p.Length)!.Value;
            int cumulativeLength = 0;
            for (int i = 0; i < keyframeEnabled[line].Count; i++)
            {
                if (keyframeEnabled[line][i]) line.Keyframes.Add((float)cumulativeLength / totalLength);
                cumulativeLength += phonemeData[line][i].Length!.Value;
            }
        }

        private void PrepareCurves(DialogueLine line)
        {
            line.Options.PitchModifier = new AnimationCurve();
            line.Options.PitchModifier.AddKey(0, 1);
            line.Options.PitchModifier.AddKey(1, 1);
            line.Options.SpeedModifier = new AnimationCurve();
            line.Options.SpeedModifier.AddKey(0, 1);
            line.Options.SpeedModifier.AddKey(1, 1);
            InsertKeyframes(line);
        }

        private void InsertKeyframes(DialogueLine line)
        {
            int totalLength = phonemeData[line].Sum(p => p.Length)!.Value;
            int[] cumulativeLengths = new int[phonemeData[line].Length];
            int l = 0;
            for (int i = 0; i < phonemeData[line].Length; i++)
            {
                cumulativeLengths[i] = l;
                l += phonemeData[line][i].Length!.Value;
            }

            for (int i = 0; i < keyframeEnabled[line].Count; i++)
            { 
                if (keyframeEnabled[line][i])
                {
                    float time = (float)cumulativeLengths[i] / totalLength;
                    line.Options.PitchModifier.AddKey(time, line.Options.PitchModifier.Evaluate(time));
                    line.Options.SpeedModifier.AddKey(time, line.Options.SpeedModifier.Evaluate(time));
                }
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
            EditorGUILayout.LabelField("Stress", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Keyframe", EditorStyles.boldLabel);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            int wordIndex = 0;
            string cleanedInput = SamUtils.CleanInput(cachedText[line]);
            string[] words = SamUtils.SplitWords(cleanedInput).ToArray();
            for (int i = 0; i < phonemeData[line].Length; i++)
            {
                PhonemeData data = phonemeData[line][i];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField(data.PhonemeName);
                EditorGUILayout.LabelField(data.Length.ToString());
                EditorGUILayout.LabelField(data.Stress.ToString());

                if (data.WordStart && !keyframeEnabled[line].ContainsKey(i))
                {
                    keyframeEnabled[line][i] = true;
                    wordBoundaries[line] ??= new List<int>();
                    wordBoundaries[line].Add(i);
                }
                
                keyframeEnabled[line].TryGetValue(i, out bool enabled);
                keyframeEnabled[line][i] = EditorGUILayout.Toggle(enabled);

                EditorGUILayout.LabelField(data.WordStart ? words[wordIndex++] : "");

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUIUtility.labelWidth = w;
        }
    }
}