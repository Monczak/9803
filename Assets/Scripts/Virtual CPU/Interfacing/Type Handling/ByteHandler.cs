namespace NineEightOhThree.VirtualCPU.Interfacing.TypeHandling
{
    public class ByteHandler : IBindableTypeHandler
    {
        public int Bytes => 1;

        public object Deserialize(string serializedValue) => byte.Parse(serializedValue);

        public object Parse(string str) => str.StartsWith("0x") ? byte.Parse(str[2..], System.Globalization.NumberStyles.HexNumber) : byte.Parse(str);

        public string Serialize(object value) => value.ToString();

        public byte[] ToBytes(object value) => new byte[] { (byte)value };
        public object FromBytes(byte[] bytes) => bytes[0];
    }
}
