using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NineEightOhThree.Utilities;
using SamSharp;
using SamSharp.Parser;
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
        
        [field: SerializeField] public List<PhonemeData> PhonemeData { get; set; }

        public List<int> WordIndexes => SamUtils.GetWordIndexes(SamUtils.CleanInput(Text)).ToList();
        public List<string> Words => SamUtils.SplitWords(SamUtils.CleanInput(Text)).ToList();
    }
}