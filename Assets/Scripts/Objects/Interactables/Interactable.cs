using System;
using System.Collections.Generic;
using UnityEngine;

namespace NineEightOhThree.Objects.Interactables
{
    [RequireComponent(typeof(Collider2D))]
    public class Interactable : MonoBehaviour
    {
        private new Collider2D collider;

        public delegate void InteractHandler(Collider2D origin);
        public event InteractHandler OnInteracted, OnStoppedInteracting;

        private void Awake()
        {
            collider = GetComponent<Collider2D>();
            ColliderCache.Instance.Register(collider, this);
        }

        private void OnDestroy()
        {
            ColliderCache.Instance.Unregister(collider, this);
        }

        public void Interact(Collider2D origin)
        {
            OnInteracted?.Invoke(origin);
        }
        
        public void StopInteracting(Collider2D origin)
        {
            OnStoppedInteracting?.Invoke(origin);
        }
    }
}