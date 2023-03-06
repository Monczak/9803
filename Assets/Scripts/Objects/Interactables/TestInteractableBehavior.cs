using System;
using NineEightOhThree.Audio;
using NineEightOhThree.Dialogues;
using NineEightOhThree.Inventories;
using NineEightOhThree.Managers;
using NineEightOhThree.VirtualCPU;
using UnityEngine;

namespace NineEightOhThree.Objects.Interactables
{
    public class TestInteractableBehavior : InteractableBehavior
    {
        private Inventory inventory;

        public Dialogue dialogue;
        
        private void Awake()
        {
            Initialize();

            inventory = GetComponent<Inventory>();
        }

        private void OnDestroy()
        {
            Destroy();
        }

        protected override void Interact(Collider2D origin)
        {
            Debug.Log($"Hello {origin.gameObject.name}");

            inventory.AddItemStack(ItemStack.Of(ItemRegistry.GetItem(1), 10));
            
            DialogueManager.Instance.StartDialogue(dialogue);
            
            CPU.Instance.SetIrq();
        }
    }
}