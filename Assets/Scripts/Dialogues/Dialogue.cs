using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace NineEightOhThree.Dialogues
{
    [CreateAssetMenu]
    public class Dialogue : ScriptableObject
    {
        [field: SerializeField] public List<DialogueLine> Lines { get; private set; }
        
        [field: SerializeField] public bool LockPlayerControls { get; set; }
    }
}