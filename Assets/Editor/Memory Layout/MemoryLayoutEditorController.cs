using System.Collections.Generic;
using NineEightOhThree.VirtualCPU.Interfacing;
using UnityEngine;

namespace NineEightOhThree.Editor.MemoryLayout
{
    public class MemoryLayoutEditorController
    {
        public List<Bindable> Bindables { get; private set; }

        public void GetAllBindablesInScene()
        {
            Bindables = new List<Bindable>();
            foreach (MemoryBindableBehavior behavior in GameObject.FindObjectsOfType<MemoryBindableBehavior>())
            {
                Bindables.AddRange(behavior.bindables);
            }
            
            foreach (Bindable bindable in Bindables)
                Debug.Log(bindable);
        }
    }
}