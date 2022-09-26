using UnityEngine;

namespace NineEightOhThree.Math
{
    public class MathExtensions
    {
        public static float Quantize(float x, int steps)
        {
            return Mathf.Round(x * steps) / steps;
        }
        
        public static float QuantizeDown(float x, int steps)
        {
            return Mathf.Floor(x * steps) / steps;
        }
        
        public static Vector2 Quantize(Vector2 v, int steps)
        {
            return new Vector2(Quantize(v.x, steps), Quantize(v.y, steps));
        }
        
        public static Vector2 QuantizeDown(Vector2 v, int steps)
        {
            return new Vector2(QuantizeDown(v.x, steps), QuantizeDown(v.y, steps));
        }

        
        public static Vector2 Abs(Vector2 v)
        {
            return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
        }

        public static Vector2 SwapXY(Vector2 v)
        {
            return new Vector2(v.y, v.x);
        }
    }
}