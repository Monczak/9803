using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }

        public string Serialize()
        {
            throw new NotImplementedException();
        }

        public object Deserialize(string str)
        {
            throw new NotImplementedException();
        }
    }
}