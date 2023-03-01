using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NineEightOhThree.Audio;
using NineEightOhThree.Dialogues;
using NineEightOhThree.Threading;
using NineEightOhThree.UI.Dialogue;
using UnityEngine;

namespace NineEightOhThree.Managers
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }
        
        public DialogueTextController dialogueTextController;

        private Dialogue currentDialogue;
        private Queue<DialogueLine> lineQueue;

        private void Awake()
        {
            Instance ??= this;
            if (Instance != this) Destroy(gameObject);
            
            SpeechManager.Instance.OnSpeechSynthesized += OnSpeechSynthesized;
            dialogueTextController.OnNextDialogueLine += OnNextDialogueLine;
        }

        private void OnNextDialogueLine(object sender, EventArgs e)
        {
            ShowNextLine();
        }

        private void ShowNextLine()
        {
            if (lineQueue.TryDequeue(out DialogueLine line))
            {
                StartDialogueLine(line);
            }
            else
            {
                EndDialogue();
            }
        }

        private void EndDialogue()
        {
            dialogueTextController.EndDialogue();
            SpeechManager.Instance.StopSpeech();
            dialogueTextController.Disable();
            
            if (currentDialogue.LockPlayerControls)
            {
                GameManager.Instance.Player.EnableControls();
            }
        }

        private void OnSpeechSynthesized(object sender, SpeechInfo e)
        {
            dialogueTextController.SpeechInfo = e;
        }

        public void StartDialogue(Dialogue dialogue)
        {
            currentDialogue = dialogue;
            
            lineQueue = new Queue<DialogueLine>(currentDialogue.Lines);
            
            dialogueTextController.Setup();

            if (currentDialogue.LockPlayerControls)
            {
                GameManager.Instance.Player.DisableControls();
            }
            
            ShowNextLine();
        }

        public async Task StartDialogueLineAsync(DialogueLine line)
        {
            SpeechManager.Instance.StopSpeech();
            await Task.Run(() => SpeechManager.Instance.SpeakDialogueLineAsync(line));

            dialogueTextController.Enable();
            dialogueTextController.StartDialogueLine(line);
        }

        public void StartDialogueLine(DialogueLine line)
        {
            UnityDispatcher.Instance.Run(() => StartDialogueLineAsync(line));
        }
    }
}