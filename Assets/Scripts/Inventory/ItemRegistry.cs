using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NineEightOhThree.Inventory
{
    public static class ItemRegistry
    {
        private static Dictionary<byte, Item> itemsById;

        public static void RegisterItems()
        {
            itemsById = new Dictionary<byte, Item>();
            Item[] items = Resources.LoadAll<Item>("Items");
            foreach (Item item in items)
            {
                itemsById[item.id] = item;
            }
            
            Debug.Log($"Registered {items.Length} items");
        }

        public static Item GetItem(byte id)
        {
            if (itemsById.ContainsKey(id))
                return itemsById[id];
            throw new InvalidItemException(id);
        }
    }
}