using System;
using UnityEngine;

namespace NineEightOhThree.Rendering
{
    [Serializable]
    public class EffectProperty
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public float Value { get; private set; }

        public EffectProperty(string name, float value)
        {
            Name = name;
            Value = value;
        }
    }
}