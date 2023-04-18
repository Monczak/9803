using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NineEightOhThree.Rendering
{
    [Serializable]
    public class EffectAnimationList : IEnumerable<EffectAnimation>
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public List<EffectAnimation> PropertyAnimations { get; private set; }
        
        public IEnumerator<EffectAnimation> GetEnumerator()
        {
            return PropertyAnimations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}