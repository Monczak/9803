using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NineEightOhThree.Audio;
using NineEightOhThree.Dialogues;
using NineEightOhThree.Utilities;
using TMPro;
using UnityEngine;

namespace NineEightOhThree.UI.Dialogue
{
    public class DialogueTextController : MonoBehaviour
    {
        public TMP_Text text;
        
        public SpeechInfo SpeechInfo { get; set; }
        
        private float letterTimer;
        private float[] wordTimings;
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (letterTimer > 0)
            {
                letterTimer -= Time.deltaTime;
            }
            else
            {
                
            }
        }

        // TODO: Show text character by character, interpolating between words and syncing to speech
        public void StartDialogueLine(DialogueLine line)
        {
            Debug.Log($"{SpeechInfo.WordTimings.Count} words");

            Debug.Log($"{SpeechInfo.LengthSeconds} {SpeechInfo.NumSamples}");

            for (int i = 0; i < SpeechInfo.WordTimings.Count; i++)
            {
                Debug.Log($"{line.Words[i]} {(float)SpeechInfo.WordTimings[i].start / SpeechInfo.SampleRate} {(float)SpeechInfo.WordTimings[i].end / SpeechInfo.SampleRate}");
            }
            
            Debug.Log($"Timings: {string.Join(", ", wordTimings.Select(t => t.ToString("F3")))}");
        }
    }
}

