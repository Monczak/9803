using System;
using System.Collections;
using System.Collections.Generic;
using NineEightOhThree.Rendering;
using UnityEngine;
using UnityEngine.UI;


namespace NineEightOhThree.Rendering
{
    [ExecuteInEditMode]
    public class UIEffectRenderer : MonoBehaviour
    {
        public List<Effect> effects;

        public RenderTexture input;
        private RenderTexture output;
        
        private RawImage rawImage;
        
        private static readonly int FXStrength = Shader.PropertyToID("_FX_Strength");

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

        // Update is called once per frame
        private void Update()
        {
            RenderTexture bufferA = RenderTexture.GetTemporary(input.width, input.height, input.depth, input.format);
            RenderTexture bufferB = RenderTexture.GetTemporary(input.width, input.height, input.depth, input.format);
            
            Graphics.Blit(input, bufferA);

            int appliedEffects = 0;

            foreach (var effect in effects)
            {
                if (!effect.enabled) continue;

                effect.MatCopy.SetFloat(FXStrength, effect.strength);

                if (appliedEffects % 2 == 0)
                    Graphics.Blit(bufferA, bufferB, effect.MatCopy, pass: 0);  // Thank you cr4y for saving me from days of pain
                else
                    Graphics.Blit(bufferB, bufferA, effect.MatCopy, pass: 0);

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

