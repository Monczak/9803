using System;

namespace NineEightOhThree.VirtualCPU.Interfacing
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    internal class BindableTypeAttribute : Attribute
    {
        public BindableType type;
        public Type objectType;
        
        public BindableTypeAttribute(BindableType type)
        {
            this.type = type;
        }

        public BindableTypeAttribute(Type objectType)
        {
            type = BindableType.Object;
            this.objectType = objectType;
        }
    }
}
