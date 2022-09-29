using System;
using JetBrains.Annotations;
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

        public static Vector2 Rotate(Vector2 v, float angle)
        {
            return new Vector2(v.x * Mathf.Cos(angle) - v.y * Mathf.Sin(angle),
                v.x * Mathf.Sin(angle) + v.y * Mathf.Cos(angle));
        }
        
        public static Vector2 RotateDegrees(Vector2 v, float angle)
        {
            angle *= Mathf.Deg2Rad;
            return new Vector2(v.x * Mathf.Cos(angle) - v.y * Mathf.Sin(angle),
                v.x * Mathf.Sin(angle) + v.y * Mathf.Cos(angle));
        }

        public static double Solve(Func<double, double> function, double initialGuess = 0f, int maxIterations = 100)
        {
            const double h = 0.0001;
            const double acceptableError = 0.0000001;
            double guess = initialGuess;

            for (int i = 0; i < maxIterations; i++)
            {
                double y = function(guess);
                if (System.Math.Abs(y) < acceptableError)
                    break;

                double slope = (function(guess + h) - y) / h;
                double step = y / slope;
                guess -= step;
            }

            return guess;
        }

        public static Vector2 Solve2D(Func<Vector2, Vector2> function, Vector2 initialGuess, int maxIterations = 100)
        {
            Vector2 h = Vector2.one * 0.0001f;
            const double acceptableError = 0.0000001;
            Vector2 guess = initialGuess;
            
            for (int i = 0; i < maxIterations; i++)
            {
                Vector2 v = function(guess);
                if (Abs(v).magnitude < acceptableError)
                    break;

                Vector2 gradient = (function(guess + h) - v) / h;
                Vector2 step = v / gradient;
                guess -= step;
            }

            return guess;
        }
    }
}