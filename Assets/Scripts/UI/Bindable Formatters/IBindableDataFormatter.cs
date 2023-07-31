using NineEightOhThree.VirtualCPU.Interfacing;

namespace NineEightOhThree.UI.BindableFormatters
{
    public interface IBindableDataFormatter
    {
        (string line, ushort address)[] Format(Bindable bindable, bool hexValues = true);
    }
}