using System;
using UnityEngine;

namespace NineEightOhThree.Managers
{
    public class ContactFilters : MonoBehaviour
    {
        public static ContactFilters Instance { get; private set; }

        public static ContactFilter2D WallFilter => Instance.wallFilter;
        
        public ContactFilter2D wallFilter;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            if (Instance != this) Destroy(gameObject);
        }
    }
}