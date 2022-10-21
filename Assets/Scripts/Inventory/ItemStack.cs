using System;
using NineEightOhThree.VirtualCPU.Interfacing;
using UnityEngine;

namespace NineEightOhThree.Inventory
{
    public class ItemStack : ScriptableObject, IBindableObject<ItemStack>
    {
        public Item itemType;
        public byte size;

        private ItemStack Of(Item itemType, byte size)
        {
            this.itemType = itemType;
            this.size = size;

            return this;
        } 
        
        public byte[] Serialize()
        {
            return new[] { itemType.id, size };
        }

        public ItemStack Deserialize(byte[] bytes)
        {
            return CreateInstance<ItemStack>().Of(ItemRegistry.GetItem(bytes[0]), bytes[1]);
        }

        public override string ToString()
        {
            return $"{itemType.itemName} x{size}";
        }
    }
}