using System;
using NineEightOhThree.VirtualCPU.Interfacing;

namespace NineEightOhThree.UI.BindableFormatters
{
    public class FormatterAttribute : Attribute
    {
        public string formatterType;

        public FormatterAttribute(BindableType type)
        {
            formatterType = type.ToString();
        }

        public FormatterAttribute(string typeName)
        {
            formatterType = typeName;
        }
    }
}