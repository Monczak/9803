using System;
using UnityEngine;

namespace NineEightOhThree.Rendering
{
    [Serializable]
    public class EffectAnimation
    {
        [field: SerializeField] public Effect Effect { get; set; }
        [field: SerializeField] public string PropertyName { get; set; }

        public EffectAnimation()
        {
            Effect = null;
            PropertyName = null;
        }

        public bool Valid => Effect is not null && Effect.HasMaterial && PropertyName is not null;
        
        [SerializeField] private AnimationCurve animationCurve;

        public AnimationCurve AnimationCurve
        {
            get => animationCurve ??= new AnimationCurve();
            private set => animationCurve = value;
        }
    }
}