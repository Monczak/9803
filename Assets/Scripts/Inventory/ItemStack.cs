using System;
using NineEightOhThree.VirtualCPU.Interfacing;
using UnityEngine;

namespace NineEightOhThree.Inventory
{
    [Serializable]
    public class ItemStack : ScriptableObject, ISerializableBindableObject
    {
        public Item itemType;
        public byte size;

        public bool Empty => size == 0;

        public ItemStack Of(Item itemType, byte size)
        {
            this.itemType = itemType;
            this.size = size;

            return this;
        } 
        
        public byte[] ToBytes()
        {
            return new[] { itemType.Id, size };
        }

        public object FromBytes(byte[] bytes)
        {
            return CreateInstance<ItemStack>().Of(ItemRegistry.GetItem(bytes[0]), bytes[1]);
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

        public override string ToString()
        {
            return $"{itemType.ItemName} x{size}";
        }

        public bool MergeWith(ItemStack other)
        {
            if (itemType != other.itemType)
                return false;

            size = (byte)Mathf.Min(size + other.size, itemType.StackSize);
            other.size = (byte)Mathf.Max(size + other.size - itemType.StackSize, 0);
            return true;
        }
    }
}