using System.Collections.Generic;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU.Interfacing
{
    [System.Serializable]
    public class MemoryBindableBehavior : MonoBehaviour
    {
        [HideInInspector]
        public List<Bindable> bindables;

        protected void Awake()
        {
            foreach (Bindable bindable in bindables)
            {
                CPU.Instance.BindableManager.RegisterBindable(bindable);
            }
        }

        protected void OnDestroy()
        {
            foreach (Bindable bindable in bindables)
            {
                CPU.Instance.BindableManager.UnregisterBindable(bindable);
            }
        }

        // Update is called once per frame
        protected void Update()
        {

        }
    }
}