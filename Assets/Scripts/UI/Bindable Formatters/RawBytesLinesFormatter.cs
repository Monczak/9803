using System.Collections.Generic;
using NineEightOhThree.VirtualCPU.Interfacing;

namespace NineEightOhThree.UI.BindableFormatters
{
    [Formatter("Raw Bytes (Lines)")]
    public class RawBytesLinesFormatter : IBindableDataFormatter
    {
        public (string line, ushort address)[] Format(Bindable bindable, bool hexValues = true)
        {
            byte[] bytes = bindable.GetBytes();
            ushort[] addresses = bindable.addresses;

            List<(string line, ushort address)> output = new();
            
            for (int i = 0; i < bindable.Bytes; i++)
            {
                output.Add(($"{bytes[i]:X2}", addresses[i]));
            }

            return output.ToArray();
        }
    }
}