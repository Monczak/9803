using NineEightOhThree.VirtualCPU.Interfacing;
using UnityEngine;

namespace NineEightOhThree.Inventories
{
    public class Inventory : MemoryBindableBehavior
    {
        [BindableType(typeof(ItemStackList)), HideInInspector]
        public Bindable itemStackList;

        private ItemStackList ItemStackList => itemStackList.GetValue<ItemStackList>();

        public bool AddItemStack(ItemStack stack)
        {
            bool result = false;
            for (int i = 0; i < ItemStackList.stackCount; i++)
            {
                bool merged = ItemStackList.ItemStacks[i].MergeWith(stack);

                if (merged && stack.Empty)
                {
                    result = true;
                    break;
                }
            }

            itemStackList.ForceUpdate();
            return result;
        }
    }
}