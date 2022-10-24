namespace NineEightOhThree.VirtualCPU.Interfacing
{
    public interface ISerializableBindableObject
    {
        public byte[] ToBytes();
        public virtual object FromBytes(byte[] bytes) => null;

        public string Serialize();
        public virtual object Deserialize(string str) => null;
        
        public int Bytes { get; }
    }
    
    public interface ISerializableBindableObject<out T> : ISerializableBindableObject where T : ISerializableBindableObject
    {
        public new T FromBytes(byte[] bytes);
        
        public new T Deserialize(string str);
    }
}