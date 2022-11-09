using System;
using NineEightOhThree.VirtualCPU.Interfacing;
using UnityEngine;

namespace NineEightOhThree.Inventories
{
    [Serializable]
    public class ItemStack : ISerializableBindableObject
    {
        public Item itemType;
        public byte size;

        public bool Empty => itemType == Item.Nothing || size == 0;

        public ItemStack Make(Item itemType, byte size)
        {
            this.itemType = itemType;
            this.size = size;

            return this;
        }

        public static ItemStack Of(Item itemType, byte size) => new ItemStack().Make(itemType, size);
        
        public byte[] ToBytes()
        {
            return new[] { itemType.Id, size };
        }

        public object FromBytes(byte[] bytes)
        {
            return new ItemStack().Make(ItemRegistry.GetItem(bytes[0]), bytes[1]);
        }

        public string Serialize()
        {
            return $"{itemType.Id} {size}";
        }

        public object Deserialize(string str)
        {
            string[] strs = str.Split(" ");
            byte[] bytes = { byte.Parse(strs[0]), byte.Parse(strs[1]) };
            return FromBytes(bytes);
        }

        public int Bytes => 2;
        public bool IsPointer => false;

        public override string ToString()
        {
            return $"{itemType.ItemName} x{size}";
        }

        public bool MergeWith(ItemStack other)
        {
            if (Empty)
                itemType = other.itemType;
            
            if (itemType != other.itemType && !(itemType == Item.Nothing || other.itemType == Item.Nothing))
                return false;
            
            if (other.itemType == Item.Nothing)
                return false;

            int oldSize = size;
            size = (byte)Mathf.Min(size + other.size, itemType.StackSize);
            other.size = (byte)Mathf.Max(oldSize + other.size - itemType.StackSize, 0);
            return true;
        }
    }
}