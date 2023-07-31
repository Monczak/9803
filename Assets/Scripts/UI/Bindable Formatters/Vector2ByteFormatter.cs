using NineEightOhThree.VirtualCPU.Interfacing;

namespace NineEightOhThree.UI.BindableFormatters
{
    [Formatter(BindableType.Vector2Byte)]
    public class Vector2ByteFormatter : IBindableDataFormatter
    {
        public (string line, ushort address)[] Format(Bindable bindable, bool hexValues = true)
        {
            byte[] bytes = bindable.GetBytes();
            return new[]
            {
                ($"X: {bytes[0].ToString(hexValues ? "X2" : "")}", bindable.addresses[0]),
                ($"Y: {bytes[1].ToString(hexValues ? "X2" : "")}", bindable.addresses[1]),
            };
        }
    }
}