using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace NineEightOhThree.Rendering
{
    [ExecuteInEditMode]
    public class UIEffectRenderer : MonoBehaviour
    {
        public RenderTexture input;
        private RenderTexture output;

        public List<Effect> effects;

        [Range(0f, 1f)] public float animationTime;

        private RawImage rawImage;
        
        private static readonly int FXStrength = Shader.PropertyToID("_FX_Strength");

        private Dictionary<string, int> propertyIDs;

        private void Awake()
        {
            output = new RenderTexture(input)
            {
                name = $"{input.name} (Effect)",
                filterMode = FilterMode.Point
            };

            rawImage = GetComponent<RawImage>();
            rawImage.texture = output;
        }

        private void GetPropertyID(EffectAnimation effectAnimation)
        {
            propertyIDs[effectAnimation.VariableName] = Shader.PropertyToID(effectAnimation.VariableName);
        }

        private void Update()
        {
            RenderTexture bufferA = RenderTexture.GetTemporary(input.width, input.height, input.depth, input.format);
            RenderTexture bufferB = RenderTexture.GetTemporary(input.width, input.height, input.depth, input.format);
            
            Graphics.Blit(input, bufferA);

            int appliedEffects = 0;

            foreach (var effect in effects)
            {
                if (!effect.enabled) continue;

                effect.Material.SetFloat(FXStrength, effect.strength);

                foreach (var effectAnimation in effect.Animations)
                {
                    if (propertyIDs is null) propertyIDs = new Dictionary<string, int>();
                    if (!propertyIDs.ContainsKey(effectAnimation.VariableName)) GetPropertyID(effectAnimation);
                    
                    effect.Material.SetFloat(propertyIDs[effectAnimation.VariableName], 
                        effectAnimation.AnimationCurve.Evaluate(animationTime) * (effectAnimation.VariableName == "_FX_Strength" ? effect.strength : 1));
                }
                
                if (appliedEffects % 2 == 0)
                    Graphics.Blit(bufferA, bufferB, effect.Material, pass: 0);  // Thank you cr4y for saving me from days of pain
                else
                    Graphics.Blit(bufferB, bufferA, effect.Material, pass: 0);

                appliedEffects++;
            }

            Graphics.Blit(appliedEffects % 2 == 0 ? bufferA : bufferB, output);

            RenderTexture.ReleaseTemporary(bufferA);
            RenderTexture.ReleaseTemporary(bufferB);
        }

        private void OnDestroy()
        {
            output.Release();
        }
    }
}

