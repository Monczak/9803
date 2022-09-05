using NineEightOhThree.VirtualCPU.Interfacing;
using UnityEngine;

namespace NineEightOhThree
{
    public class TestBindableBehavior : MemoryBindableBehavior
    {
        [BindableType(BindableType.Byte)]
        [HideInInspector]
        public Bindable testBindable;

        private new void Awake()
        {
            base.Awake();
        }

        private new void Update()
        {
            base.Update();

            // Debug.Log(testValue.GetValue<byte>());
        }
    }
}
