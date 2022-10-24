namespace NineEightOhThree.VirtualCPU.Interfacing
{
    public interface ISerializableBindableObject
    {
        public byte[] ToBytes();
        public object FromBytes(byte[] bytes);

        public string Serialize();
        public object Deserialize(string str);
        
        public int Bytes { get; }

    }
}