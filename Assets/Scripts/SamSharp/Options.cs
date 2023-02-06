using System;
using NineEightOhThree.Utilities;
using UnityEngine;

namespace SamSharp
{
    [Serializable]
    public class Options
    {
        [field: SerializeField] public byte Pitch { get; set; }
        [field: SerializeField] public byte Mouth { get; set; }
        [field: SerializeField] public byte Throat { get; set; }
        [field: SerializeField] public byte Speed { get; set; }
        [field: SerializeField] public bool SingMode { get; set; }

        [field: SerializeField, Curve(0, 0, 1, 1, true)] public AnimationCurve PitchModifier { get; set; }
        [field: SerializeField, Curve(0, 0, 1, 1, true)] public AnimationCurve SpeedModifier { get; set; }

        public Options(byte pitch = 64, byte mouth = 128, byte throat = 128, byte speed = 72, bool singMode = false)
        {
            Pitch = pitch;
            Mouth = mouth;
            Throat = throat;
            Speed = speed;
            SingMode = singMode;

            PitchModifier = new AnimationCurve();
            SpeedModifier = new AnimationCurve();
        }
    }
}