using UnityEngine;

namespace NineEightOhThree.Utilities
{
    public static class CurveUtils
    {
        public static float Integrate(AnimationCurve curve, float startTime, float endTime, int steps) {
            float total = 0;
            float step = (endTime - startTime) / steps;
            for (int i = 0; i < steps; i++) {
                float t1 = startTime + i * step;
                float t2 = startTime + (i + 1) * step;
                total += (curve.Evaluate(t1) + curve.Evaluate(t2)) * step / 2;
            }
            return total;
        }
    }
}