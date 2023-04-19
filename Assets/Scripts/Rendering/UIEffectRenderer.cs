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
        public List<EffectAnimationList> animations;
        
        public int animationIndex;
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

        public void InitializeEffects(bool destructive = false)
        {
            foreach (var effect in effects)
            {
#if UNITY_EDITOR
                effect.InitializeProperties(destructive);
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

        private int GetPropertyID(string propertyName)
        {
            UpdatePropertyID(propertyName);
            return propertyIDs[propertyName];
        }
        
        private void Update()
        {
            RenderTexture bufferA = RenderTexture.GetTemporary(input.width, input.height, input.depth, input.format);
            RenderTexture bufferB = RenderTexture.GetTemporary(input.width, input.height, input.depth, input.format);
            
            Graphics.Blit(input, bufferA);

            int appliedEffects = 0;

            foreach (var effect in effects)
            {
                if (!effect.enabled || !effect.HasMaterial) continue;

                effect.SetupPropertyDict();
                foreach (var effectProperty in effect.Properties.Values)
                {
                    UpdatePropertyID(effectProperty.Name);
                }

                SetBasePropertyValues(effect);
            }

            if (animationIndex >= 0 && animationIndex < animations.Count)
            {
                foreach (var effectAnimation in animations[animationIndex])
                {
                    if (!effectAnimation.Valid) continue;
                    
                    float value = effectAnimation.Effect.Material.GetFloat(GetPropertyID(effectAnimation.PropertyName));
                    value *= effectAnimation.AnimationCurve.Evaluate(animationTime);
                    effectAnimation.Effect.Material.SetFloat(GetPropertyID(effectAnimation.PropertyName), value);
                }
            }

            foreach (var effect in effects) 
            {
                if (!effect.enabled || !effect.HasMaterial) continue;
                
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

        private void SetBasePropertyValues(Effect effect)
        {
            foreach (var effectProperty in effect.Properties.Values)
            {
                float value = effectProperty.Value;
                effect.Material.SetFloat(propertyIDs[effectProperty.Name], value);
            }
        }

        private void OnDestroy()
        {
            output.Release();
        }
    }
}

