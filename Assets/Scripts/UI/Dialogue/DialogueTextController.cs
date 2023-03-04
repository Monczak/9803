using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NineEightOhThree.Audio;
using NineEightOhThree.Dialogues;
using NineEightOhThree.Managers;
using NineEightOhThree.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NineEightOhThree.UI.Dialogue
{
    public class DialogueTextController : MonoBehaviour
    {
        [Header("Components")]
        public GameObject container;
        public TMP_Text text;
        public NextLineIconController nextLineIconController;

        [Header("Anchoring")]
        public bool anchorUp;
        public bool autoAnchorUp;
        public float playerYThreshold;
        

        private RectTransform rectTransform;
        private TMPTextFormatter formatter;

        private Transform playerTransform;

        public SpeechInfo SpeechInfo { get; set; }
        
        private float letterTimer;
        private int[] shownCharCount;

        private List<int> interpolationKeyframes;

        private const int TicksPerSecond = 30;

        private int tickIndex;
        private bool showingLine;
        private bool showingDialogue;

        private Dialogues.Dialogue currentDialogue;
        private LineEvent currentLine;

        private UIControls controls;
        private bool skipDialogue;

        public event EventHandler OnDialogueLineFinished;
        public event EventHandler OnNextDialogueLine;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            
            formatter = text.GetComponent<TMPTextFormatter>();
            controls = new UIControls();
            
            controls.Game.SkipDialogue.performed += OnSkipDialoguePerformed;

            playerTransform = GameObject.FindWithTag("Player").transform;
            
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
            if (autoAnchorUp)
            {
                anchorUp = GameManager.Instance.GameCamera.WorldToViewportPoint(playerTransform.position).y <
                           playerYThreshold;
            }
            
            SetPosition();
            
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
                    RevealText();

                    if (tickIndex >= shownCharCount.Length - 1)
                    {
                        showingLine = false;
                        OnDialogueLineFinished?.Invoke(this, EventArgs.Empty);
                        nextLineIconController.Show();
                    }
                }
                else
                {
                    letterTimer = 0;

                    if (skipDialogue)
                    {
                        OnNextDialogueLine?.Invoke(this, EventArgs.Empty);
                        nextLineIconController.Hide();
                    }
                }
            }
            
            skipDialogue = false;
        }

        public void Enable()
        {
            container.SetActive(true);
        }

        public void Disable()
        {
            container.SetActive(false);
        }
        
        private void SetPosition()
        {
            rectTransform.anchorMin = new Vector2(0.5f, anchorUp ? 1f : 0f);
            rectTransform.anchorMax = new Vector2(0.5f, anchorUp ? 1f : 0f);
            
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, anchorUp ? -rectTransform.rect.height : 0f);
        }

        private void RevealText()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<color=#ffffffff>").Append(currentLine.Text[..shownCharCount[tickIndex]]).Append("</color>");
            builder.Append("<color=#ffffff00>").Append(currentLine.Text[shownCharCount[tickIndex]..]).Append("</color>");
            text.text = builder.ToString();
        }

        public void EndDialogue()
        {
            showingDialogue = false;
        }

        // TODO: Formatting, layout etc.
        private void SetupFormatting()
        {
            /*formatter.Begin(text)
                .Color(0, text.text.Length, new Color32(255, 255, 255, 0))
                .Color(0, shownCharCount[tickIndex], new Color32(255, 255, 255, 255))
                .Apply();*/
        }

        public void Setup()
        {
            text.text = "";
            nextLineIconController.Setup();
            nextLineIconController.HideImmediate();
        }

        public void StartDialogueLine(LineEvent line)
        {
            currentLine = line;
            
            PrepareCharCountArray(line);
            InterpolateCharCounts();

            tickIndex = 0;
            
            SetupFormatting();
            
            showingDialogue = true;
            showingLine = true;
        }

        private void PrepareCharCountArray(LineEvent line)
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

