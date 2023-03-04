using System;

namespace NineEightOhThree.Dialogues
{
    [Serializable]
    public abstract class DialogueEvent
    {
        public abstract void Handle();
        
        public abstract string EditorTitle { get; }
    }
}