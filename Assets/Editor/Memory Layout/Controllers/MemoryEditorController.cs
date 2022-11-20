using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NineEightOhThree.Editor.MemoryLayout.Controllers
{
    public class MemoryEditorController
    {
        private List<Label> memoryCellLabels, addressLabels;

        public void Initialize(VisualElement root)
        {
            memoryCellLabels = new List<Label>();
            addressLabels = new List<Label>();
            
            List<VisualElement> rows = root.Query<VisualElement>("MemoryCellRow").Build().ToList();
            foreach (VisualElement row in rows)
            {
                List<Label> labels = row.Query<Label>("MemoryCellLabel").Build().ToList();
                Label addressLabel = row.Q<Label>("Address");
                
                memoryCellLabels.AddRange(labels);
                addressLabels.Add(addressLabel);
            }
            
            Debug.Log(memoryCellLabels.Count);
        }
    }
}