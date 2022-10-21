using UnityEngine;

namespace NineEightOhThree.Inventory
{
    [CreateAssetMenu]
    public class Item : ScriptableObject
    {
        public byte id;
        public byte stackSize = 255;
        public string itemName;
        public Texture2D sprite;
    }
}