using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NineEightOhThree.Rendering
{
    [Serializable]
    public class Effect
    {
        [SerializeField] private Material sourceMaterial;
        public bool enabled;

        private Material matCopy;

        public bool HasMaterial => sourceMaterial != null;
        public Material Material => HasMaterial ? matCopy && matCopy.name == sourceMaterial.name ? matCopy : matCopy = new Material(sourceMaterial) : null;

        public List<EffectProperty> propertyList;
        public Dictionary<string, EffectProperty> Properties { get; private set; }

        public string Name => Material != null ? Material.name : null;

#if UNITY_EDITOR
        public void InitializeProperties(bool destructive = false)
        {
            if (!destructive)
            {
                if (propertyList is not null && propertyList.Count > 0) return;
            }
            
            Properties = null;
        
            propertyList = new List<EffectProperty>();

            if (Material is null)
                return;

            int propertyCount = ShaderUtil.GetPropertyCount(Material.shader);
            for (int i = 0; i < propertyCount; i++)
            {
                string name = ShaderUtil.GetPropertyName(Material.shader, i);
                if (!name.StartsWith("_FX_")) continue;
                
                var property = new EffectProperty(name, Material.GetFloat(name));
                propertyList.Add(property);
            }
        }
#endif

        public void SetupPropertyDict()
        {
            if (Properties is not null) return;
            Properties = new Dictionary<string, EffectProperty>();
            foreach (var property in propertyList)
            {
                Properties.Add(property.Name, property);
            }
        }
    }
}