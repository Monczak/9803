using NineEightOhThree.Math;

namespace NineEightOhThree.VirtualCPU.Interfacing.TypeHandling
{
    public class Vector2ByteHandler : IBindableTypeHandler
    {
        public int Bytes => 2;

        public string[] AddressNames => new[] { "X", "Y" };

        public object Deserialize(string serializedValue) => new Vector2Byte(byte.Parse(serializedValue.Split("|")[0]), byte.Parse(serializedValue.Split("|")[1]));

        public object Parse(string str) => new Vector2Byte(byte.Parse(str.Split(" ")[0]), byte.Parse(str.Split(" ")[1]));

        public string Serialize(object value) => $"{((Vector2Byte)value).x}|{((Vector2Byte)value).y}";

        public byte[] ToBytes(object value) => new byte[] { ((Vector2Byte)value).x, ((Vector2Byte)value).y };
        public object FromBytes(byte[] bytes) => new Vector2Byte(bytes[0], bytes[1]);
    }
}