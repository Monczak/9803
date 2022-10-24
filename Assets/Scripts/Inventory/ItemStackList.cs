using System;
using System.Collections.Generic;
using System.Text;
using NineEightOhThree.VirtualCPU.Interfacing;
using UnityEngine;

namespace NineEightOhThree.Inventory
{
    [Serializable]
    public class ItemStackList : ScriptableObject, ISerializableBindableObject
    {
        public List<ItemStack> itemStacks;

        public int Bytes => itemStacks.Count * CreateInstance<ItemStack>().Bytes;

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[Bytes];
            for (int i = 0; i < itemStacks.Count; i++)
            {
                byte[] itemStackBytes = itemStacks[i].ToBytes();
                bytes[i * 2] = itemStackBytes[0];
                bytes[i * 2 + 1] = itemStackBytes[1];
            }

            return bytes;
        }

        public object FromBytes(byte[] bytes)
        {
            ItemStackList list = CreateInstance<ItemStackList>();
            list.itemStacks = new List<ItemStack>();

            for (int i = 0; i < bytes.Length; i += 2)
            {
                list.itemStacks.Add((ItemStack)CreateInstance<ItemStack>().FromBytes(bytes[i..(i + 1)]));
            }

            return list;
        }

        public string Serialize()
        {
            StringBuilder builder = new StringBuilder();
            foreach (ItemStack stack in itemStacks)
            {
                builder.Append(stack.Serialize()).Append("|");
            }

            return builder.ToString();
        }

        public object Deserialize(string str)
        {
            string[] serializedItems = str.Split("|");
            byte[] bytes = new byte[serializedItems.Length * 2];
            for (int i = 0; i < serializedItems.Length; i++)
            {
                string[] itemData = serializedItems[i].Split(" ");
                bytes[i * 2] = byte.Parse(itemData[0]);
                bytes[i * 2 + 1] = byte.Parse(itemData[1]);
            }

            return FromBytes(bytes);
        }
    }
}