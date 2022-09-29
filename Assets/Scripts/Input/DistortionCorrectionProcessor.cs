using NineEightOhThree.Math;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NineEightOhThree.Input
{
    #if UNITY_EDITOR
    [InitializeOnLoad]
    #endif
    public class DistortionCorrectionProcessor : InputProcessor<Vector2>
    {
        public float centerX, centerY;
        public float strengthX, strengthY;
        public float offsetX, offsetY;
        public float scale;
        public bool enabled;
        
        private Vector2 Center => new(centerX, centerY);
        private Vector2 Strength => new(strengthX, strengthY);
        private Vector2 Offset => new(offsetX, offsetY);
        
        #if UNITY_EDITOR
        static DistortionCorrectionProcessor()
        {
            Initialize();
        }
        #endif
        
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            InputSystem.RegisterProcessor<DistortionCorrectionProcessor>();
        }

        private Vector2 ToUV(Vector2 screenPos)
        {
            return screenPos / new Vector2(Screen.width, Screen.height);
        }
        
        private Vector2 FromUV(Vector2 uv)
        {
            return uv * new Vector2(Screen.width, Screen.height);
        }

        private Vector2 Spherize(Vector2 uv)
        {
            Vector2 delta = uv - Center;
            float delta2 = Vector2.Dot(delta, delta);
            float delta4 = delta2 * delta2;
            Vector2 deltaOffset = delta4 * Strength;
            return uv + delta * deltaOffset + Offset;
        }

        private Vector2 TransformUV(Vector2 uv)
        {
            return Spherize(uv) * (1.0f / scale) + Vector2.one * (1.0f / scale - 1) * -0.5f;
        }

        private Vector2 InverseTransformUV(Vector2 uv)
        {
            return MathExtensions.Solve2D(v => uv - TransformUV(v), uv);
        } 
        
        public override Vector2 Process(Vector2 value, InputControl control)
        {
            return enabled ? FromUV(TransformUV(ToUV(value))) : value;
        }
    }
}