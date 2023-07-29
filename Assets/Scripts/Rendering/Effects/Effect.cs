using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NineEightOhThree.Rendering.Effects
{
    [CreateAssetMenu(menuName = "Effects/Effect")]
    public class Effect : ScriptableObject
    {
        [SerializeField] private Material sourceMaterial;
        private Material previousMaterial;
        
        private Material matCopy;

        public bool HasMaterial => sourceMaterial != null;
        public Material Material => 
            HasMaterial
                ? matCopy && matCopy.name == sourceMaterial.name 
                    ? matCopy 
                    : matCopy = new Material(sourceMaterial) 
                : null;

        private Dictionary<string, EffectProperty> properties;
        public Dictionary<string, EffectProperty> Properties
        {
            get
            {
                if (properties is null || previousMaterial != sourceMaterial) InitializeProperties();
                previousMaterial = sourceMaterial;
                return properties;
            }
            private set => properties = value;
        }
        
        public string Name => Material != null ? Material.name : null;

        private void InitializeProperties()
        {
            properties = new Dictionary<string, EffectProperty>();

            if (Material is null)
                return;

            int propertyCount = ShaderUtil.GetPropertyCount(Material.shader);
            for (int i = 0; i < propertyCount; i++)
            {
                string propertyName = ShaderUtil.GetPropertyName(Material.shader, i);
                if (!propertyName.StartsWith("_FX_")) continue;
                
                var property = new EffectProperty(propertyName, Material.GetFloat(propertyName));
                properties.Add(propertyName, property);
            }
        }

        public void UpdateShader()
        {
            foreach (EffectProperty property in Properties.Values)
            {
                Material.SetFloat(property.Name, property.Value);
            }
        }
    }
}