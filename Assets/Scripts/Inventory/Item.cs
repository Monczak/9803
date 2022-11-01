using System.Collections.Generic;
using UnityEngine;

namespace NineEightOhThree.Inventory
{
    [CreateAssetMenu]
    public class Item : ScriptableObject
    {
        [field: SerializeField] public byte Id { get; private set; }
        [field: SerializeField] public byte StackSize { get; private set; } = 255;
        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public Texture2D Sprite { get; private set;  }

        public override bool Equals(object other)
        {
            if (other is Item o)
                return o.Id == Id;
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Item i1, Item i2) => i1 is not null && i2 is not null && i1.Equals(i2);
        public static bool operator !=(Item i1, Item i2) => !(i1 == i2);
    }
}