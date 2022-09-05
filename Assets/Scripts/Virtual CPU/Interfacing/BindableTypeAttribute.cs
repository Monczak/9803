using System;

namespace NineEightOhThree.VirtualCPU.Interfacing
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    internal class BindableTypeAttribute : Attribute
    {
        public BindableType type;

        public BindableTypeAttribute(BindableType type)
        {
            this.type = type;
        }
    }
}
