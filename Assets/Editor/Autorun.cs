using NineEightOhThree.Dialogues;
using NineEightOhThree.Inventories;
using UnityEditor;

namespace NineEightOhThree.Editor
{
    #if UNITY_EDITOR
    [InitializeOnLoad]
    public class Autorun
    {
        // TODO: Refactor all registries to use a base ObjectRegistry class and fire all ObjectRegistries here?
        static Autorun()
        {
            if (ItemRegistry.IsNull())
                ItemRegistry.RegisterItems();
            
            if (DialogueEventRegistry.IsNull())
                DialogueEventRegistry.RegisterEvents();
            
        }
    }
    #endif
}