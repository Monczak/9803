using System.Collections.Generic;
using UnityEngine;

namespace NineEightOhThree.Dialogues
{
    [CreateAssetMenu]
    public class Dialogue : ScriptableObject
    {
        [field: SerializeField] public List<DialogueLine> Lines { get; private set; }
    }
}