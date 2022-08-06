namespace NineEightOhThree.VirtualCPU.Utilities
{
    public class BitUtils
    {
        public static void SetBit(ref byte n, byte bit, bool set)
        {
            byte x = (byte)(set ? 1 : 0);
            n ^= (byte)((-x ^ n) & (1 << bit));
        }

        public static byte GetBit(byte n, byte bit)
        {
            return (byte)((n & (1 << bit)) >> bit);
        }

        public static ushort FromLittleEndian(byte b1, byte b2)
        {
            return (ushort)(b1 + b2 << 8);
        }
    }
}
