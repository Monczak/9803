using System.Collections.Generic;
using NineEightOhThree.VirtualCPU;
using UnityEngine;
using UnityEngine.UIElements;

namespace NineEightOhThree.Editor.MemoryLayout.Controllers
{
    public class MemoryEditorController
    {
        private List<Label> memoryCellLabels, addressLabels;

        private VisualElement editorContainer;
        
        private Memory memory;

        private bool shiftHeld, ctrlHeld;

        public ushort FirstAddress { get; set; }
        private const ushort MinAddress = 0x0000;
        private const ushort MaxAddress = 0xFFFF;
        private const ushort AddressStep = 16;

        public void Initialize(VisualElement root)
        {
            memoryCellLabels = new List<Label>();
            addressLabels = new List<Label>();

            editorContainer = root.Q<VisualElement>("MemoryEditorContainer");
            
            List<VisualElement> rows = editorContainer.Query<VisualElement>("MemoryCellRow").Build().ToList();
            foreach (VisualElement row in rows)
            {
                List<Label> labels = row.Query<Label>("MemoryCellLabel").Build().ToList();
                Label addressLabel = row.Q<Label>("Address");
                
                memoryCellLabels.AddRange(labels);
                addressLabels.Add(addressLabel);
            }

            for (int i = 0; i < memoryCellLabels.Count; i++)
            {
                var label = memoryCellLabels[i];
                label.parent.name = $"Label{i}";
            }

            editorContainer.RegisterCallback<WheelEvent>(OnWheel, TrickleDown.TrickleDown);

            RegisterClickHandlers();
        }

        private void RegisterClickHandlers()
        {
            for (int i = 0; i < memoryCellLabels.Count; i++)
            {
                int i1 = i;
                memoryCellLabels[i].RegisterCallback<ClickEvent>(evt =>
                {
                    Debug.Log($"hallo {i1} have been clicked");
                }, TrickleDown.TrickleDown);
            }
        }

        private void OnWheel(WheelEvent evt)
        {
            ushort step = (ushort)(AddressStep * (shiftHeld ? 16 : 1) * (ctrlHeld ? 256 : 1));
            
            if (evt.delta.y > 0)
            {
                if (FirstAddress < MaxAddress - 256)
                    FirstAddress += step;
            }
            else
            {
                if (FirstAddress > MinAddress)
                    FirstAddress -= step;
            }
        }

        public void Update()
        {
            for (int i = 0; i < addressLabels.Count; i++)
            {
                ushort address = (ushort)(FirstAddress + i * 16);
                addressLabels[i].text = address.ToString("X4");
            }
        }
    }
}