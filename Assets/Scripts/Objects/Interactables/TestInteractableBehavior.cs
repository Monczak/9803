using System;
using UnityEngine;

namespace NineEightOhThree.Objects.Interactables
{
    public class TestInteractableBehavior : InteractableBehavior
    {
        private void Awake()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            Destroy();
        }

        protected override void Interact(Collider2D origin)
        {
            Debug.Log($"Hello {origin.gameObject.name}");
        }
    }
}