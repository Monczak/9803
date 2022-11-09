using NineEightOhThree.Inventories;
using UnityEditor;

namespace NineEightOhThree
{
    [InitializeOnLoad]
    public class Autorun
    {
        static Autorun()
        {
            if (ItemRegistry.IsNull())
                ItemRegistry.RegisterItems();
        }
    }
}