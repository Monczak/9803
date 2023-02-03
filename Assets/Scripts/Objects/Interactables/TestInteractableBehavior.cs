using System;
using NineEightOhThree.Audio;
using NineEightOhThree.Inventories;
using UnityEngine;

namespace NineEightOhThree.Objects.Interactables
{
    public class TestInteractableBehavior : InteractableBehavior
    {
        private Inventory inventory;
        
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
            
            SpeechManager.Instance.Speak("Hello, my name is SAM.");
        }
    }
}