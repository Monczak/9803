using System;
using System.Collections.Generic;
using UnityEngine;

namespace NineEightOhThree.Rendering
{
    [Serializable]
    public class Effect
    {
        [SerializeField] private Material sourceMaterial;
        public bool enabled;
        public float strength;

        private Material matCopy;

        public Material Material => matCopy && matCopy.name == sourceMaterial.name ? matCopy : matCopy = new Material(sourceMaterial);
        
        [field: SerializeField] public List<EffectAnimation> Animations { get; private set; }
    }
}