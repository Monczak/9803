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
        private Queue<DialogueEvent> eventQueue;

        private void Awake()
        {
            Instance ??= this;
            if (Instance != this) Destroy(gameObject);
            
            SpeechManager.Instance.OnSpeechSynthesized += OnSpeechSynthesized;
            dialogueTextController.OnNextDialogueLine += OnNextDialogueLine;
        }

        private void OnNextDialogueLine(object sender, EventArgs e)
        {
            HandleNextEvent();
        }

        private void HandleNextEvent()
        {
            while (true)
            {
                if (eventQueue.TryDequeue(out DialogueEvent theEvent))
                {
                    theEvent.Handle();
                    if (!Attribute.IsDefined(theEvent.GetType(), typeof(NotifyCompleteAttribute))) continue;
                }
                else
                {
                    EndDialogue();
                }

                break;
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
            
            eventQueue = new Queue<DialogueEvent>(currentDialogue.Events);
            
            dialogueTextController.Setup();

            if (currentDialogue.LockPlayerControls)
            {
                GameManager.Instance.Player.DisableControls();
            }
            
            HandleNextEvent();
        }

        public async Task StartDialogueLineAsync(LineEvent line)
        {
            SpeechManager.Instance.StopSpeech();
            await Task.Run(() => SpeechManager.Instance.SpeakDialogueLineAsync(line));

            dialogueTextController.Enable();
            dialogueTextController.StartDialogueLine(line);
        }

        public void StartDialogueLine(LineEvent line)
        {
            UnityDispatcher.Instance.Run(() => StartDialogueLineAsync(line));
        }
    }
}