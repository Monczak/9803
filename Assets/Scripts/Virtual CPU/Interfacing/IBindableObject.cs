namespace NineEightOhThree.VirtualCPU.Interfacing
{
    public interface IBindableObject<out T>
    {
        public byte[] Serialize();
        public T Deserialize(byte[] bytes);
    }
}