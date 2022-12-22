using System;
using System.Collections.Generic;
using UnityEngine;

namespace NineEightOhThree.Objects
{
    public class ColliderCache : MonoBehaviour
    {
        public static ColliderCache Instance { get; private set; }

        private Dictionary<Collider2D, Dictionary<Type, Component>> cache = new();

        private void Awake()
        {
            Instance = this;
        }

        public void Register<T>(Collider2D origin, T component) where T : Component
        {
            if (!cache.ContainsKey(origin))
                cache.Add(origin, new Dictionary<Type, Component>());

            Logger.Log($"Registered {component.GetType().Name} for {origin.gameObject.name}");
            cache[origin][component.GetType()] = component;
            
        }

        public T Get<T>(Collider2D col) where T : Component
        {
            if (!cache.ContainsKey(col))
                return null;
            if (!cache[col].ContainsKey(typeof(T)))
                return null;
            return (T)cache[col][typeof(T)];
        }

        public bool TryGet<T>(Collider2D col, out T component) where T : Component
        {
            component = null;
            if (!cache.ContainsKey(col))
                return false;
            if (!cache[col].ContainsKey(typeof(T)))
                return false;
            component = (T)cache[col][typeof(T)];
            return true;
        }

        public void Unregister<T>(Collider2D origin, T component) where T : Component
        {
            cache[origin].Remove(typeof(T));
        }

        public void Unregister(Collider2D origin)
        {
            cache.Remove(origin);
        }
    }
}