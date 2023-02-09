using System;
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
        }

        public void StartDialogueLine(DialogueLine line)
        {
            dialogueTextController.StartDialogueLine(line);
        }
    }
}