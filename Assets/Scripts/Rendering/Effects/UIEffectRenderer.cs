using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace NineEightOhThree.Rendering.Effects
{
    [ExecuteInEditMode]
    public class UIEffectRenderer : MonoBehaviour
    {
        public RenderTexture input;
        private RenderTexture output;

        public List<Effect> effects;

        private RawImage rawImage;

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

        private void Update()
        {
            RenderTexture bufferA = RenderTexture.GetTemporary(input.width, input.height, input.depth, input.format);
            RenderTexture bufferB = RenderTexture.GetTemporary(input.width, input.height, input.depth, input.format);
            
            Graphics.Blit(input, bufferA);

            bool fromAToB = true;

            foreach (var effect in effects) 
            {
                if (effect is null || !effect.HasMaterial) continue;
                
                effect.UpdateShader();
                
                if (fromAToB)
                    Graphics.Blit(bufferA, bufferB, effect.Material, pass: 0);  // Thank you cr4y for saving me from days of pain
                else
                    Graphics.Blit(bufferB, bufferA, effect.Material, pass: 0);

                fromAToB ^= true;
            }

            Graphics.Blit(fromAToB ? bufferA : bufferB, output);

            RenderTexture.ReleaseTemporary(bufferA);
            RenderTexture.ReleaseTemporary(bufferB);
        }

        private void OnDestroy()
        {
            output.Release();
        }
    }
}

