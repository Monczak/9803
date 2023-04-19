using System;
using UnityEngine;

namespace NineEightOhThree.Rendering
{
    [Serializable]
    public class EffectAnimation
    {
        [field: SerializeField] public Effect Effect { get; set; }
        [field: SerializeField] public EffectProperty Property { get; set; }

        public EffectAnimation()
        {
            Effect = null;
            Property = null;
        }

        public string PropertyName => Property.Name;

        public bool Valid => Effect is not null && Effect.HasMaterial && Property?.Name != null;
        
        [SerializeField] private AnimationCurve animationCurve;

        public AnimationCurve AnimationCurve
        {
            get => animationCurve ??= new AnimationCurve();
            private set => animationCurve = value;
        }
    }
}