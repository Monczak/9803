using System;
using System.Collections.Generic;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU.Interfacing
{
    [Serializable]
    public class MemoryBindableBehavior : MonoBehaviour
    {
        [HideInInspector]
        public List<Bindable> bindables;

        protected virtual void Awake()
        {
            foreach (Bindable bindable in bindables)
            {
                CPU.Instance.BindableManager.RegisterBindable(bindable);
            }
        }

        protected virtual void OnDestroy()
        {
            foreach (Bindable bindable in bindables)
            {
                CPU.Instance.BindableManager.UnregisterBindable(bindable);
            }
        }

        // Update is called once per frame
        protected virtual void Update()
        {

        }

        protected virtual void LateUpdate()
        {
            foreach (Bindable bindable in bindables)
            {
                bindable.dirty = false;
            }
        }
    }
}