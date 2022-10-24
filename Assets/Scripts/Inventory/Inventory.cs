using NineEightOhThree.VirtualCPU.Interfacing;
using UnityEngine;

namespace NineEightOhThree.Inventory
{
    public class Inventory : MemoryBindableBehavior
    {
        [BindableType(typeof(ItemStackList)), HideInInspector]
        public Bindable itemStackList;
    }
}