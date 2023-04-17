using System;
using System.Collections.Generic;
using UnityEngine;

namespace NineEightOhThree.Rendering
{
    [Serializable]
    public class EffectAnimationList
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public List<EffectAnimation> Animations { get; private set; }
    }
}