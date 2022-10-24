using System;
using NineEightOhThree.VirtualCPU.Interfacing;
using UnityEngine;

namespace NineEightOhThree.Inventory
{
    public class ItemStack : ScriptableObject, ISerializableBindableObject
    {
        public Item itemType;
        public byte size;

        private ItemStack Of(Item itemType, byte size)
        {
            this.itemType = itemType;
            this.size = size;

            return this;
        } 
        
        public byte[] ToBytes()
        {
            return new[] { itemType.id, size };
        }

        public object FromBytes(byte[] bytes)
        {
            return CreateInstance<ItemStack>().Of(ItemRegistry.GetItem(bytes[0]), bytes[1]);
        }

        public string Serialize()
        {
            throw new NotImplementedException();
        }

        public object Deserialize(string str)
        {
            throw new NotImplementedException();
        }

        public int Bytes => 2;

        public override string ToString()
        {
            return $"{itemType.itemName} x{size}";
        }
    }
}