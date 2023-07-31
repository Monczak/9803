using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NineEightOhThree.UI.BindableFormatters;
using NineEightOhThree.VirtualCPU.Interfacing;
using UnityEngine;
using UnityEngine.Serialization;

namespace NineEightOhThree.UI.Tooltips
{
    public class TooltipDataProvider : MonoBehaviour, ISerializationCallbackReceiver
    {
        public Dictionary<string, TooltipDataRules> dataRules;
        private List<Bindable> bindables;

        [HideInInspector] public new Collider2D collider;

        [SerializeField, HideInInspector] private string serializedRuleData;

        private void Awake()
        {
            collider = GetComponent<Collider2D>();
            FindBindables();
        }

        public void OnBeforeSerialize()
        {
            serializedRuleData = JsonConvert.SerializeObject(dataRules);
        }

        public void OnAfterDeserialize()
        {
            dataRules = JsonConvert.DeserializeObject<Dictionary<string, TooltipDataRules>>(serializedRuleData);
        }

        public void FindBindables()
        {
            bindables = new List<Bindable>(GetAllBindables());
            dataRules ??= new Dictionary<string, TooltipDataRules>();
            
            foreach (Bindable bindable in bindables)
            {
                dataRules.TryAdd(bindable.ShortKeyName, new TooltipDataRules());
            }
        }

        private Bindable[] GetAllBindables()
        {
            bindables = new List<Bindable>();
            
            var behaviors = GetComponents<MemoryBindableBehavior>();
            foreach (var behavior in behaviors)
            {
                bindables.AddRange(behavior.bindables);
            }

            return bindables.ToArray();
        }

        private TooltipDataRules GetRules(Bindable bindable) => dataRules[bindable.ShortKeyName];

        public TooltipData GetData(Bindable bindable, bool hexValues = true)
        {
            TooltipDataRules rules = GetRules(bindable);

            var lines = BindableFormatterRegistry.GetFormatter(rules.formatterIndex).Format(bindable, hexValues);
            
            TooltipData data = new(rules.name, lines);
            return data;
        }

        public TooltipData[] GetAllData(bool hexValues = true)
        {
            List<TooltipData> data = new();
            foreach (Bindable bindable in bindables)
            {
                data.Add(GetData(bindable, hexValues));
            }

            return data.ToArray();
        }
    }
}