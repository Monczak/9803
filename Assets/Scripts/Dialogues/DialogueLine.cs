using System;
using System.Collections.Generic;
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
        
        [field: SerializeField] public List<float> Keyframes { get; set; }
        [field: SerializeField] public List<float> WordBoundaryKeyframes { get; set; }
    }
}