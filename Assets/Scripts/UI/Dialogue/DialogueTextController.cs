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
        private bool showing;

        private void Awake()
        {
            formatter = text.GetComponent<TMPTextFormatter>();
        }

        // Start is called before the first frame update
        private void Start()
        {
        
        }

        // Update is called once per frame
        private void Update()
        {
            if (showing)
            {
                SetupFormatting();

                letterTimer += Time.deltaTime;
                tickIndex = (int)(letterTimer * TicksPerSecond);
                
                if (tickIndex >= shownCharCount.Length)
                    showing = false;
            }
            else
            {
                letterTimer = 0;
            }
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
            PrepareCharCountArray(line);
            InterpolateCharCounts();

            tickIndex = 0;

            text.text = line.Text;
            showing = true;
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

