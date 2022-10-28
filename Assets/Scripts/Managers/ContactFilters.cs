using System;
using UnityEngine;

namespace NineEightOhThree.Managers
{
    public class ContactFilters : MonoBehaviour
    {
        public static ContactFilters Instance { get; private set; }

        public static ContactFilter2D WallFilter => Instance.wallFilter;
        public static ContactFilter2D InteractableFilter => Instance.interactableFilter;
        
        public ContactFilter2D wallFilter;
        public ContactFilter2D interactableFilter;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            if (Instance != this) Destroy(gameObject);
        }
    }
}