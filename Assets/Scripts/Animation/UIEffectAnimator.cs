using System;
using System.Collections.Generic;
using NineEightOhThree.Rendering.Effects;
using UnityEngine;

namespace NineEightOhThree.Animation
{
    [RequireComponent(typeof(UIEffectRenderer)), ExecuteInEditMode]
    public class UIEffectAnimator : MonoBehaviour
    {
        [SerializeField] private EffectAnimation effectAnimation;
        public EffectAnimation EffectAnimation
        {
            get => effectAnimation;
            set
            {
                UpdateEffectRenderer();
                effectAnimation = value;
            }
        }
        
        [Range(0f, 1f)] public float animationTime;

        private UIEffectRenderer effectRenderer;
        
        private void Awake()
        {
            effectRenderer = GetComponent<UIEffectRenderer>();
            UpdateEffectRenderer();
        }

        private void Update()
        {
            if (EffectAnimation is null) return;
            
            foreach (AnimatedProperty property in effectAnimation.AnimatedProperties)
            {
                property.EffectProperty.Value = property.AnimationCurve.Evaluate(animationTime);
            }
        }

        private void UpdateEffectRenderer()
        {
            effectRenderer.effects = new List<Effect>(effectAnimation.usedEffects);
        }
    }
}