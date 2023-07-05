using System;
using System.Collections.Generic;
using UnityEngine;

namespace NineEightOhThree.Rendering.Effects
{
    [CreateAssetMenu(menuName = "Effects/Animation")]
    public class EffectAnimation : ScriptableObject
    {
        [field: SerializeField, HideInInspector] public List<AnimatedProperty> AnimatedProperties { get; private set; }

        public List<Effect> usedEffects;
    }
}