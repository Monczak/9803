using System;
using System.Reflection;

namespace NineEightOhThree.VirtualCPU.Interfacing
{
    [Serializable]
    public class BindableMeta
    {
        public FieldInfo FieldInfo;
        public ushort Address;

        public BindableMeta(FieldInfo fieldInfo, ushort address)
        {
            FieldInfo = fieldInfo;
            Address = address;
        }

        public void SetAddress(ushort address)
        {
            Address = address;
        }
    }
}
