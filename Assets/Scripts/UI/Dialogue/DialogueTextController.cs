using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NineEightOhThree.Audio;
using NineEightOhThree.Dialogues;
using NineEightOhThree.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

namespace NineEightOhThree.UI.Dialogue
{
    public class DialogueTextController : MonoBehaviour
    {
        public TMP_Text text;

        private TMPTextFormatter formatter;
        
        public SpeechInfo SpeechInfo { get; set; }
        
        private float letterTimer;
        private int[] shownCharCount;

        private List<int> interpolationKeyframes;

        private const int TicksPerSecond = 30;

        private int tickIndex;
        private bool showingLine;
        private bool showingDialogue;

        private Dialogues.Dialogue currentDialogue;
        private DialogueLine currentLine;

        private UIControls controls;
        private bool skipDialogue;

        public event EventHandler OnDialogueLineFinished;
        public event EventHandler OnNextDialogueLine;

        private void Awake()
        {
            formatter = text.GetComponent<TMPTextFormatter>();
            controls = new UIControls();
            
            controls.Game.SkipDialogue.performed += OnSkipDialoguePerformed;

            controls.Enable();
        }

        private void OnSkipDialoguePerformed(InputAction.CallbackContext obj)
        {
            if (showingDialogue)
            {
                skipDialogue = obj.ReadValue<float>() > 0;
            }
        }

        // Start is called before the first frame update
        private void Start()
        {
        
        }

        // Update is called once per frame
        private void Update()
        {
            if (showingDialogue)
            {
                if (showingLine)
                {
                    if (currentLine.Skippable && skipDialogue && tickIndex > 2)
                    {
                        tickIndex = shownCharCount.Length - 1;
                    }
                    else
                    {
                        letterTimer += Time.deltaTime;
                        tickIndex = (int)(letterTimer * TicksPerSecond);
                    }

                    SetupFormatting();

                    if (tickIndex >= shownCharCount.Length - 1)
                    {
                        showingLine = false;
                        OnDialogueLineFinished?.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    letterTimer = 0;

                    if (skipDialogue)
                    {
                        OnNextDialogueLine?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            
            skipDialogue = false;
        }

        public void EndDialogue()
        {
            showingDialogue = false;
        }

        // TODO: Formatting, layout etc.
        private void SetupFormatting()
        {
            formatter.Begin(text)
                .Color(0, text.text.Length, new Color32(255, 255, 255, 0))
                .Color(0, shownCharCount[tickIndex], new Color32(255, 255, 255, 255))
                .Apply();
        }

        public void StartDialogueLine(DialogueLine line)
        {
            currentLine = line;
            
            PrepareCharCountArray(line);
            InterpolateCharCounts();

            tickIndex = 0;

            text.text = line.Text;
            SetupFormatting();
            
            showingDialogue = true;
            showingLine = true;
        }

        private void PrepareCharCountArray(DialogueLine line)
        {
            shownCharCount = new int[(int)(SpeechInfo.LengthSeconds * TicksPerSecond)];
            interpolationKeyframes = new List<int>();

            int wordIndex = 0;
            int currentlyShownChars = 0;
            bool inRange = false;
            var words = SamUtils.GetWordMatches(line.Text).ToList();

            for (int i = 0; i < shownCharCount.Length; i++)
            {
                int currentTime = (int)((float)i / TicksPerSecond * SpeechInfo.SampleRate);

                shownCharCount[i] = currentlyShownChars;

                if (currentTime < SpeechInfo.WordTimings[wordIndex].start)
                {
                    // Debug.Log($"{i} Waiting for word: {words[wordIndex].Value}");
                }
                else if (currentTime >= SpeechInfo.WordTimings[wordIndex].start &&
                         currentTime <= SpeechInfo.WordTimings[wordIndex].end)
                {
                    // Debug.Log($"{i} In range of word: {words[wordIndex].Value}");

                    if (!inRange)
                    {
                        currentlyShownChars = words[wordIndex].Index + words[wordIndex].Length;
                        interpolationKeyframes.Add(i);
                    }

                    inRange = true;
                }
                else if (currentTime > SpeechInfo.WordTimings[wordIndex].end)
                {
                    // Debug.Log($"{i} Reached end of word: {words[wordIndex].Value}");

                    inRange = false;
                    wordIndex++;

                    interpolationKeyframes.Add(i);

                    if (wordIndex >= words.Count)
                    {
                        for (int j = i; j < shownCharCount.Length; j++)
                            shownCharCount[j] = currentlyShownChars;
                        break;
                    }
                }
            }
            shownCharCount[^1] = currentlyShownChars;

            if (inRange)
            {
                if (interpolationKeyframes[^1] != shownCharCount.Length - 1)
                    interpolationKeyframes.Add(shownCharCount.Length - 1);
            }
        }

        private void InterpolateCharCounts()
        {
            for (int i = 0; i < interpolationKeyframes.Count - 1; i++)
            {
                (int start, int end) = (interpolationKeyframes[i],
                    interpolationKeyframes[i + 1]);
                (int startValue, int endValue) = (shownCharCount[interpolationKeyframes[i]],
                    shownCharCount[interpolationKeyframes[i + 1]]);
                for (int j = 0; j < end - start; j++)
                {
                    shownCharCount[start + j] = Mathf.CeilToInt(Mathf.Lerp(startValue, endValue, (float)j / (end - start)));
                }
            }
        }
    }
}

