using System;

namespace NineEightOhThree.VirtualCPU.Interfacing
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class MemoryBindableAttribute : Attribute
    {
        public ushort Address;
    }
}
