using NineEightOhThree.Inventory;
using UnityEditor;

namespace NineEightOhThree
{
    [InitializeOnLoad]
    public class Autorun
    {
        static Autorun()
        {
            ItemRegistry.RegisterItems();
        }
    }
}