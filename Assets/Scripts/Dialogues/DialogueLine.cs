using System;
using SamSharp;
using UnityEngine;

namespace NineEightOhThree.Dialogues
{
    [Serializable]
    public class DialogueLine
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public string PhoneticText { get; set; }
        
        [field: SerializeField] public Options Options { get; set; }
    }
}