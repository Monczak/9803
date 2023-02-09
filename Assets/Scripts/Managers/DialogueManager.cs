using System;
using System.Threading.Tasks;
using NineEightOhThree.Audio;
using NineEightOhThree.Dialogues;
using NineEightOhThree.UI.Dialogue;
using UnityEngine;

namespace NineEightOhThree.Managers
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }
        
        public DialogueTextController dialogueTextController;

        private void Awake()
        {
            Instance ??= this;
            if (Instance != this) Destroy(gameObject);
            
            SpeechManager.Instance.OnSpeechSynthesized += OnSpeechSynthesized;
        }

        private void OnSpeechSynthesized(object sender, SpeechInfo e)
        {
            dialogueTextController.SpeechInfo = e;
        }

        public async void StartDialogueLineAsync(DialogueLine line)
        {
            await SpeechManager.Instance.SpeakDialogueLineAsync(line);
            dialogueTextController.StartDialogueLine(line);
        }

        public void StartDialogueLine(DialogueLine line)
        {
            Task.Run(() => StartDialogueLineAsync(line));
        }
    }
}