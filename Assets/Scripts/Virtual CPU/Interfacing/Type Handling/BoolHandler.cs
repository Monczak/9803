namespace NineEightOhThree.VirtualCPU.Interfacing.TypeHandling
{
    public class BoolHandler : IBindableTypeHandler
    {
        public int Bytes => 1;

        public object Deserialize(string serializedValue) => bool.Parse(serializedValue);

        public object Parse(string str) => str != "0" && str.Trim() != "" && str.Trim().ToLower() != "false";

        public string Serialize(object value) => value.ToString();

        public byte[] ToBytes(object value) => (bool)value ? new byte[] { 0x01 } : new byte[] { 0x00 };
        public object FromBytes(byte[] bytes) => bytes[0] != 0x00;
    }
}
