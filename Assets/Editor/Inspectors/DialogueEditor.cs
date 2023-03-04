using System;
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

        private Dictionary<DialogueEvent, bool> foldOuts;

        private Sam sam;
        
        private Dictionary<LineEvent, PhonemeData[]> phonemeData;
        private Dictionary<LineEvent, bool> showPhonemes;
        private Dictionary<LineEvent, Dictionary<int, bool>> keyframeEnabled;
        private Dictionary<LineEvent, List<int>> wordBoundaries;
        private Dictionary<LineEvent, string> cachedText;

        private void OnEnable()
        {
            dialogue = target as Dialogue;
            if (dialogue is null)
                return;

            dialogue.Events ??= new List<DialogueEvent>();

            foldOuts ??= new Dictionary<DialogueEvent, bool>();
            
            phonemeData ??= new Dictionary<LineEvent, PhonemeData[]>();
            showPhonemes = new Dictionary<LineEvent, bool>();
            keyframeEnabled ??= new Dictionary<LineEvent, Dictionary<int, bool>>();
            wordBoundaries ??= new Dictionary<LineEvent, List<int>>();
            cachedText ??= new Dictionary<LineEvent, string>();

            if (foldOuts.Count == 0)
            {
                foreach (DialogueEvent theEvent in dialogue.Events)
                {
                    foldOuts[theEvent] = false;
                    if (theEvent is LineEvent line)
                        PrepareLine(line);
                }
            }

            sam = new Sam();
        }

        private void PrepareLine(LineEvent line)
        {
            phonemeData[line] = null;
            showPhonemes[line] = false;
            keyframeEnabled[line] = new Dictionary<int, bool>();
            wordBoundaries[line] = new List<int>();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);

            dialogue.LockPlayerControls = EditorGUILayout.Toggle("Lock Player Controls", dialogue.LockPlayerControls);
            
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("New: ", EditorStyles.label, GUILayout.Width(40));
            foreach (Type type in DialogueEventRegistry.EventTypes.Values)
            {
                bool pressed = GUILayout.Button(string.Join(" ", StringUtils.Beautify(type.Name).Split(" ")[..^1]));
                if (pressed)
                {
                    DialogueEvent newEvent = (DialogueEvent)Activator.CreateInstance(type);
                    switch (newEvent)
                    {
                        case LineEvent line:
                            line.Text = "New Line";
                            PrepareLine(line);
                            break;
                    }
                    
                    dialogue.Events.Add(newEvent);
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            DrawEventEditors();

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(dialogue);
            
            Undo.RecordObject(dialogue, "Edit Dialogue");
        }

        private void DrawEventEditors()
        {
            DialogueEvent toRemove = null;
            int index = -1;
            bool moveUp = false;
            bool moveDown = false;
            for (int i = 0; i < dialogue.Events.Count; i++)
            {
                var theEvent = dialogue.Events[i];
                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        foldOuts[theEvent] = EditorGUILayout.BeginFoldoutHeaderGroup(foldOuts[theEvent], theEvent.EditorTitle);
                        if (foldOuts[theEvent])
                        {
                            switch (theEvent)
                            {
                                case LineEvent line:
                                    DrawDialogueLineEditor(line);
                                    break;
                            }
                        }
                        EditorGUILayout.EndFoldoutHeaderGroup();
                        
                    }

                    if (GUILayout.Button("/\\", new GUIStyle(EditorStyles.miniButton) { fixedWidth = 25 }))
                    {
                        index = i;
                        moveUp = true;
                    }

                    if (GUILayout.Button("\\/", new GUIStyle(EditorStyles.miniButton) { fixedWidth = 25 }))
                    {
                        index = i;
                        moveDown = true;
                    }

                    if (GUILayout.Button("-", new GUIStyle(EditorStyles.miniButton) { fixedWidth = 20 }))
                        toRemove = theEvent;
                }
            }

            if (toRemove is not null) dialogue.Events.Remove(toRemove);
            if (moveUp && index > 0)
            {
                (dialogue.Events[index], dialogue.Events[index - 1]) =
                    (dialogue.Events[index - 1], dialogue.Events[index]);
            }

            if (moveDown && index < dialogue.Events.Count - 1 && index != -1)
            {
                (dialogue.Events[index], dialogue.Events[index + 1]) =
                    (dialogue.Events[index + 1], dialogue.Events[index]);
            }
        }

        private void DrawDialogueLineEditor(LineEvent line)
        {
            EditorGUILayout.PrefixLabel("Text");
            line.Text = EditorGUILayout.TextArea(line.Text, new GUIStyle(EditorStyles.textArea) {wordWrap = true});
            EditorGUILayout.LabelField($"Cleaned (dialogue): \"{SamUtils.CleanInputForDialogue(line.Text)}\"", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField($"Cleaned (SAM): \"{SamUtils.CleanInputForSam(line.Text)}\"", EditorStyles.wordWrappedLabel);
            
            try
            {
                string phoneticText = sam.TextToPhonemes(line.CleanedSamInput);
                EditorGUILayout.LabelField($"Phonetic: \"{phoneticText}\"", EditorStyles.wordWrappedLabel);
            }
            catch (Exception e)
            {
                EditorGUILayout.LabelField($"[WARN] SAM Reciter failed: {e.Message}", EditorStyles.wordWrappedLabel);
                Debug.LogError(e.ToString());
            }

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("SAM Parameters", EditorStyles.boldLabel);
            line.SamOptions.Pitch = (byte)EditorGUILayout.IntSlider("Pitch", line.SamOptions.Pitch, 0, 255);
            line.SamOptions.Mouth = (byte)EditorGUILayout.IntSlider("Mouth", line.SamOptions.Mouth, 0, 255);
            line.SamOptions.Throat = (byte)EditorGUILayout.IntSlider("Throat", line.SamOptions.Throat, 0, 255);
            line.SamOptions.Speed = (byte)EditorGUILayout.IntSlider("Speed", line.SamOptions.Speed, 1, 255);
            line.SamOptions.SingMode = EditorGUILayout.Toggle("Sing Mode", line.SamOptions.SingMode);

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Modifiers", EditorStyles.boldLabel);
            line.SamOptions.PitchModifier ??= new AnimationCurve();
            line.SamOptions.PitchModifier =
                EditorGUILayout.CurveField("Pitch", line.SamOptions.PitchModifier, Color.red, new Rect(0, 0, 1, 2));
            line.SamOptions.SpeedModifier ??= new AnimationCurve();
            line.SamOptions.SpeedModifier =
                EditorGUILayout.CurveField("Speed", line.SamOptions.SpeedModifier, Color.cyan, new Rect(0, 0, 1, 2));

            EditorGUILayout.Separator();

            line.Skippable = EditorGUILayout.Toggle("Skippable", line.Skippable);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Get Phoneme Data"))
            {
                sam.GetPhonemeDataAsync(line.CleanedSamInput).ContinueWith(t =>
                {
                    phonemeData[line] = t.Result;
                    showPhonemes[line] = true;
                    keyframeEnabled[line] = new Dictionary<int, bool>();
                    cachedText[line] = line.Text;
                    wordBoundaries[line] = new List<int>();
                    line.PhonemeData = new List<PhonemeData>(phonemeData[line]);
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

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(10);
                using (new GUILayout.VerticalScope())
                {
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
            }
        }

        private void SetWordBoundaries(LineEvent line)
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

        private void SetKeyframes(LineEvent line)
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

        private void PrepareCurves(LineEvent line)
        {
            line.SamOptions.PitchModifier = new AnimationCurve();
            line.SamOptions.PitchModifier.AddKey(0, 1);
            line.SamOptions.PitchModifier.AddKey(1, 1);
            line.SamOptions.SpeedModifier = new AnimationCurve();
            line.SamOptions.SpeedModifier.AddKey(0, 1);
            line.SamOptions.SpeedModifier.AddKey(1, 1);
            InsertKeyframes(line);
        }

        private void InsertKeyframes(LineEvent line)
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
                    line.SamOptions.PitchModifier.AddKey(time, line.SamOptions.PitchModifier.Evaluate(time));
                    line.SamOptions.SpeedModifier.AddKey(time, line.SamOptions.SpeedModifier.Evaluate(time));
                }
            }
        }
        
        private void DrawPhonemeTable(LineEvent line)
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
            string cleanedInput = SamUtils.CleanInputForSam(cachedText[line]);
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

                EditorGUILayout.LabelField(data.WordStart ? words[wordIndex] : "");
                EditorGUILayout.LabelField(data.WordEnd ? $"-- {words[wordIndex]}" : "");
                if (data.WordEnd)
                {
                    if (wordIndex >= words.Length)
                    {
                        Debug.LogWarning($"Word index {wordIndex} exceeds word count ({words.Length})");
                        wordIndex = words.Length - 1;
                    }
                    else
                        wordIndex++;
                    
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUIUtility.labelWidth = w;
        }
    }
}