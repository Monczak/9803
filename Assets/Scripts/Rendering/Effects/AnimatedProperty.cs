using System;
using System.Collections.Generic;
using UnityEngine;

namespace NineEightOhThree.Rendering.Effects
{
    [Serializable]
    public class AnimatedProperty
    {
        [field: SerializeField] public Effect Effect { get; set; }
        [field: SerializeField] public string PropertyName { get; set; }
        [field: SerializeField] public AnimationCurve AnimationCurve { get; set; }

        public AnimatedProperty(Effect effect, string propertyName)
        {
            Effect = effect;
            PropertyName = propertyName;
            AnimationCurve = new AnimationCurve();
        }

        public EffectProperty EffectProperty => Effect.Properties[PropertyName];
    }
}