namespace NineEightOhThree.VirtualCPU.Interfacing.TypeHandling
{

    public interface IBindableTypeHandler
    {
        public object Parse(string str);
        public string Serialize(object value);
        public object Deserialize(string serializedValue);

        public byte[] ToBytes(object value);
        public object FromBytes(byte[] bytes);

        public int Bytes { get; }
    }
}
