using UnityEngine;

namespace NineEightOhThree.Dialogues
{
    public class TestEvent : DialogueEvent
    {
        public override void Handle()
        {
            Debug.Log("Test Dialogue Event");
        }

        public override string EditorTitle => "Test Event";
    }
}