using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NineEightOhThree.VirtualCPU.Interfacing;
using UnityEngine;

namespace NineEightOhThree.Inventories
{
    [Serializable]
    [CreateAssetMenu]
    public class ItemStackList : ScriptableObject, ISerializableBindableObject, IEnumerable<ItemStack>
    {
        public byte stackCount;
        
        [SerializeField] private List<ItemStack> itemStacks;
        public List<ItemStack> ItemStacks
        {
            get => itemStacks ??= new List<ItemStack>();
            set => itemStacks = value;
        }
        

        public int Bytes => 1 + stackCount * new ItemStack().Bytes;
        public bool IsPointer => true;

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[Bytes];
            bytes[0] = stackCount;
            for (int i = 0; i < ItemStacks.Count; i++)
            {
                byte[] itemStackBytes = ItemStacks[i].ToBytes();
                bytes[1 + i * 2] = itemStackBytes[0];
                bytes[1 + i * 2 + 1] = itemStackBytes[1];
            }

            return bytes;
        }

        public object FromBytes(byte[] bytes)
        {
            ItemStackList list = CreateInstance<ItemStackList>();
            list.ItemStacks = new List<ItemStack>();
            list.stackCount = bytes[0];

            for (int i = 1; i < bytes.Length; i += 2)
            {
                list.ItemStacks.Add((ItemStack)new ItemStack().FromBytes(bytes[i..(i + 2)]));
            }

            return list;
        }

        public string Serialize()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(stackCount).Append("|");
            builder.AppendJoin("|", ItemStacks.Select(stack => stack.Serialize()));

            return builder.ToString();
        }

        public object Deserialize(string str)
        {
            string[] serializedItems = str.Split("|", StringSplitOptions.RemoveEmptyEntries);
            byte[] bytes = new byte[1 + (serializedItems.Length - 1) * 2];
            bytes[0] = byte.Parse(serializedItems[0]);
            for (int i = 1; i < serializedItems.Length; i++)
            {
                string[] itemData = serializedItems[i].Split(" ");
                bytes[1 + (i - 1) * 2] = byte.Parse(itemData[0]);
                bytes[1 + (i - 1) * 2 + 1] = byte.Parse(itemData[1]);
            }

            return FromBytes(bytes);
        }

        private IEnumerator<ItemStack> enumerator;
        
        public IEnumerator<ItemStack> GetEnumerator()
        {
            return enumerator ??= itemStacks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}