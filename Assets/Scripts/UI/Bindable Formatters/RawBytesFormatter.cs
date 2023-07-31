using System.Linq;
using NineEightOhThree.VirtualCPU.Interfacing;

namespace NineEightOhThree.UI.BindableFormatters
{
    [Formatter("Raw Bytes")]
    public class RawBytesFormatter : IBindableDataFormatter
    {
        public (string line, ushort address)[] Format(Bindable bindable, bool hexValues = true)
        {
            byte[] bytes = bindable.GetBytes();
            return new[]
            {
                (string.Join(" ", bytes.Select(b => b.ToString("X2"))), bindable.addresses[0])
            };
        }
    }
}