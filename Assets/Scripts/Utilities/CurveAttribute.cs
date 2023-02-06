using UnityEngine;

namespace NineEightOhThree.Utilities
{
    public class CurveAttribute : PropertyAttribute
    {
        public float PosX { get; }
        public float PosY { get; }
        
        public float RangeX { get; }
        public float RangeY { get; }

        public bool Enabled { get; }

        public CurveAttribute(float posX, float posY, float rangeX, float rangeY, bool enabled)
        {
            PosX = posX;
            PosY = posY;
            RangeX = rangeX;
            RangeY = rangeY;
            Enabled = enabled;
        }
    }
}