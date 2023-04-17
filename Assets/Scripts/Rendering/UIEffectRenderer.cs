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
            
            InitializeEffects();
        }

        private void OnEnable()
        {
            InitializeEffects();
        }

        private void InitializeEffects()
        {
            foreach (var effect in effects)
            {
#if UNITY_EDITOR
                effect.InitializeProperties();
#endif

                effect.SetupPropertyDict();
                foreach (var property in effect.Properties.Values)
                {
                    UpdatePropertyID(property.Name);
                }
            }
        }

        private void UpdatePropertyID(string propertyName)
        {
            propertyIDs ??= new Dictionary<string, int>();
            if (!propertyIDs.ContainsKey(propertyName)) propertyIDs[propertyName] = Shader.PropertyToID(propertyName);
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
                
                effect.SetupPropertyDict();
                foreach (var effectProperty in effect.Properties.Values)
                {
                    UpdatePropertyID(effectProperty.Name);
                }

                foreach (var effectProperty in effect.Properties.Values)
                {
                    float value = effectProperty.Value;
                    effect.Material.SetFloat(propertyIDs[effectProperty.Name], value);
                }

                foreach (var effectAnimation in effect.Animations)
                {
                    if (!effect.Properties.ContainsKey(effectAnimation.PropertyName)) continue;

                    float value = effect.Material.GetFloat(propertyIDs[effectAnimation.PropertyName]);
                    value *= effectAnimation.AnimationCurve.Evaluate(animationTime);
                    effect.Material.SetFloat(propertyIDs[effectAnimation.PropertyName], value);
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

