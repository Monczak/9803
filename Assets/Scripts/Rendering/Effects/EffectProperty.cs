using System;
using NineEightOhThree.Utilities;
using UnityEngine;

namespace NineEightOhThree.Rendering.Effects
{
    [Serializable]
    public class EffectProperty
    {
        [field: SerializeField] public string Name { get; private set; }
        public float Value { get; set; }

        public string NiceName => StringUtils.Beautify(Name.Split("_FX_")[^1].Replace("_", " "));

        public EffectProperty(string name, float value)
        {
            Name = name;
            Value = value;
        }
    }
}