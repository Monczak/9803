using System.Collections;
using System.Collections.Generic;
using NineEightOhThree.Audio;
using NineEightOhThree.Dialogues;
using TMPro;
using UnityEngine;

namespace NineEightOhThree.UI.Dialogue
{
    public class DialogueTextController : MonoBehaviour
    {
        public TMP_Text text;
        
        public SpeechInfo SpeechInfo { get; set; }
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        // TODO: Show text character by character, interpolating between words and syncing to speech
        public void StartDialogueLine(DialogueLine line)
        {
            Logger.Log($"Preparing speech for \"{line.Text}\"");
            
            
            Debug.Log($"{line.WordBoundaryKeyframes.Count} word boundaries");
            for (int i = 0; i < line.WordBoundaryKeyframes.Count; i++)
            {
                float keyframe = line.WordBoundaryKeyframes[i];
                Debug.Log($"{keyframe} {line.Words[i]} {line.WordIndexes[i]} - {keyframe * SpeechInfo.LengthSeconds} secs");
            }
        }
    }
}

