using NineEightOhThree.VirtualCPU.Utilities;

namespace NineEightOhThree.VirtualCPU.Interfacing.TypeHandling
{
    public class UshortHandler : IBindableTypeHandler
    {
        public int Bytes => 2;

        public string[] AddressNames => new[] { "High Byte", "Low Byte" };

        public object Deserialize(string serializedValue) => ushort.Parse(serializedValue);

        public object Parse(string str) => ushort.Parse(str);

        public string Serialize(object value) => value.ToString();

        public byte[] ToBytes(object value) => BitUtils.ToLittleEndian((ushort)value);
        public object FromBytes(byte[] bytes) => BitUtils.FromLittleEndian<ushort>(bytes);
    }
}
