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

                if (itemsById.TryGetValue(item.Id, out var value))
                {
                    Debug.LogError($"Invalid ID for item {item.ItemName} (ID {item.Id} is already taken by {value.ItemName})");
                    continue;
                }
                itemsById[item.Id] = item;
            }
            
            Debug.Log($"Registered {items.Length} items");
        }

        public static Item GetItem(byte id)
        {
            if (itemsById.TryGetValue(id, out var item))
                return item;
            throw new InvalidItemException(id);
        }
    }
}