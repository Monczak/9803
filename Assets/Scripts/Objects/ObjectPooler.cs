using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NineEightOhThree.Objects
{
    public class ObjectPooler : MonoBehaviour
    {
        [field: SerializeField] public GameObject Prefab { get; private set; }

        private List<GameObject> objects;

        public int minCapacity;
        
        public bool expandable;
        public float bonusObjectLifetime;

        private void Awake()
        {
            objects = new List<GameObject>();
            
            for (int i = 0; i < minCapacity; i++)
            {
                InstantiateObject();
            }
        }

        private GameObject InstantiateObject()
        {
            GameObject obj = Instantiate(Prefab, transform);
            obj.SetActive(false);
            objects.Add(obj);
            return obj;
        }

        public GameObject RequestObject()
        {
            GameObject obj = FindInactiveObject();
            if (obj is null && expandable)
            {
                obj = InstantiateObject();
            }

            return obj;
        }

        public GameObject[] RequestObjects(int count)
        {
            GameObject[] objs = new GameObject[count];
            for (int i = 0; i < count; i++)
                objs[i] = RequestObject();

            return objs;
        }

        private GameObject FindInactiveObject()
        {
            return objects.FirstOrDefault(obj => !obj.activeInHierarchy);
        }
    }
}