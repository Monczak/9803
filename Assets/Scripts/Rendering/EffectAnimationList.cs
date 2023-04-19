using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NineEightOhThree.Rendering
{
    [Serializable]
    public class EffectAnimationList : ICollection<EffectAnimation>
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public List<EffectAnimation> PropertyAnimations { get; private set; }

        public EffectAnimationList()
        {
            Name = "New Animation List";
            PropertyAnimations = new List<EffectAnimation>();
        }
        
        public IEnumerator<EffectAnimation> GetEnumerator()
        {
            return PropertyAnimations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(EffectAnimation item)
        {
            PropertyAnimations.Add(item);
        }

        public void Clear()
        {
            PropertyAnimations.Clear();
        }

        public bool Contains(EffectAnimation item)
        {
            return PropertyAnimations.Contains(item);
        }

        public void CopyTo(EffectAnimation[] array, int arrayIndex)
        {
            PropertyAnimations.CopyTo(array, arrayIndex);
        }

        public bool Remove(EffectAnimation item)
        {
            return PropertyAnimations.Remove(item);
        }

        public int Count => PropertyAnimations.Count;
        public bool IsReadOnly => false;
    }
}