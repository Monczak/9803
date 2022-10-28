using UnityEngine;

namespace NineEightOhThree.Objects.Interactables
{
    [RequireComponent(typeof(Interactable))]
    public abstract class InteractableBehavior : MonoBehaviour
    {
        private Interactable interactable;
        
        public void Initialize()
        {
            interactable = GetComponent<Interactable>();
            interactable.OnInteracted += Interact;
            interactable.OnStoppedInteracting += StopInteracting;
        }

        public void Destroy()
        {
            interactable.OnInteracted -= Interact;
            interactable.OnStoppedInteracting -= StopInteracting;
        }
        
        protected abstract void Interact(Collider2D origin);
        protected virtual void StopInteracting(Collider2D origin) { }
    }
}