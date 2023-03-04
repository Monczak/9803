using System.Collections.Generic;
using UnityEngine;

namespace NineEightOhThree.Dialogues
{
    [CreateAssetMenu]
    public class Dialogue : ScriptableObject
    {
        [field: SerializeField, SerializeReference] public List<DialogueEvent> Events { get; set; }
        
        [field: SerializeField] public bool LockPlayerControls { get; set; }
    }
}