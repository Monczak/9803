using NineEightOhThree.VirtualCPU.Utilities;

namespace NineEightOhThree.VirtualCPU.Interfacing.TypeHandling
{
    public class LongHandler : IBindableTypeHandler
    {
        public int Bytes => 8;

        public object Deserialize(string serializedValue) => long.Parse(serializedValue);

        public object Parse(string str) => long.Parse(str);

        public string Serialize(object value) => value.ToString();

        public byte[] ToBytes(object value) => BitUtils.ToLittleEndian((long)value);
        public object FromBytes(byte[] bytes) => BitUtils.FromLittleEndian<long>(bytes);
    }
}
