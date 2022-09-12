using NineEightOhThree.VirtualCPU.Utilities;

namespace NineEightOhThree.VirtualCPU.Interfacing.TypeHandling
{
    public class IntHandler : IBindableTypeHandler
    {
        public int Bytes => 4;

        public object Deserialize(string serializedValue) => int.Parse(serializedValue);

        public object Parse(string str) => int.Parse(str);

        public string Serialize(object value) => value.ToString();

        public byte[] ToBytes(object value) => BitUtils.ToLittleEndian((int)value);
        public object FromBytes(byte[] bytes) => BitUtils.FromLittleEndian<int>(bytes);
    }
}
