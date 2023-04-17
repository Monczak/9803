using System;
using UnityEngine;

namespace NineEightOhThree.Rendering
{
    [Serializable]
    public class EffectAnimation
    {
        [field: SerializeField] public string PropertyName { get; private set; }
        
        [SerializeField] private AnimationCurve animationCurve;
        public AnimationCurve AnimationCurve
        {
            get => animationCurve ??= new AnimationCurve();
            private set => animationCurve = value;
        }
    }
}