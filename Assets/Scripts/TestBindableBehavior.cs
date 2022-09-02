using NineEightOhThree.VirtualCPU.Interfacing;

namespace NineEightOhThree
{
    public class TestBindableBehavior : MemoryBindableBehavior
    {
        public Bindable testValue1;
        public Bindable testValue2;
        public Bindable testValue3;
        public byte testByte;

        private void Awake()
        {

        }

        private new void Update()
        {
            base.Update();

            // Debug.Log(testValue.GetValue<byte>());
        }
    }
}
