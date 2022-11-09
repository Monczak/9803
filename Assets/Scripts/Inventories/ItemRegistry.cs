using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NineEightOhThree.Inventories
{
    public static class ItemRegistry
    {
        private static Dictionary<byte, Item> itemsById;

        public static bool IsNull() => itemsById is null;

        public static void RegisterItems()
        {
            itemsById = new Dictionary<byte, Item>
            {
                [Item.Nothing.Id] = Item.Nothing
            };

            Item[] items = Resources.LoadAll<Item>("Items");
            foreach (Item item in items)
            {
                if (item.Id == Item.Nothing.Id)
                {
                    Debug.LogError($"Invalid ID for item {item.ItemName} ({Item.Nothing.Id} is reserved for Nothing)");
                    continue;
                }

                if (itemsById.ContainsKey(item.Id))
                {
                    Debug.LogError($"Invalid ID for item {item.ItemName} (ID {item.Id} is already taken by {itemsById[item.Id].ItemName})");
                    continue;
                }
                itemsById[item.Id] = item;
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