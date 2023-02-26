using NineEightOhThree.Inventories;
using UnityEditor;

namespace NineEightOhThree
{
    #if UNITY_EDITOR
    [InitializeOnLoad]
    public class Autorun
    {
        static Autorun()
        {
            if (ItemRegistry.IsNull())
                ItemRegistry.RegisterItems();
        }
    }
    #endif
}