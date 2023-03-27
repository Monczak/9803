using System;

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
            return (ushort)(b1 + (b2 << 8));
        }

        public static object FromLittleEndian<T>(params byte[] bytes)
        {
            return typeof(T) switch
            {
                var t when t == typeof(byte) => bytes[0],
                var t when t == typeof(ushort) => (ushort)(bytes[0] + (bytes[1] << 8)),
                var t when t == typeof(int) => bytes[0] + (bytes[1] << 8) + (bytes[2] << 16) + (bytes[3] << 24),
                var t when t == typeof(long) => bytes[0] + (bytes[1] << 8) + (bytes[2] << 16) + (bytes[3] << 24) + (bytes[4] << 32) + (bytes[5] << 40) + (bytes[6] << 48) + (bytes[7] << 56), // TODO: May use ints, not longs?
                _ => null,
            };
        }

        public static object FromLittleEndian<T>(ReadOnlySpan<byte> bytes)
        {
            return typeof(T) switch
            {
                var t when t == typeof(byte) => bytes[0],
                var t when t == typeof(ushort) => (ushort)(bytes[0] + (bytes[1] << 8)),
                var t when t == typeof(int) => bytes[0] + (bytes[1] << 8) + (bytes[2] << 16) + (bytes[3] << 24),
                var t when t == typeof(long) => bytes[0] + (bytes[1] << 8) + (bytes[2] << 16) + (bytes[3] << 24) + (bytes[4] << 32) + (bytes[5] << 40) + (bytes[6] << 48) + (bytes[7] << 56), // TODO: May use ints, not longs?
                _ => null,
            };
        }

        public static byte[] ToLittleEndian(ushort n)
        {
            return new byte[] { (byte)n, (byte)(n >> 8) };
        }

        public static byte[] ToLittleEndian(int n)
        {
            return new byte[] { (byte)n, (byte)(n >> 8), (byte)(n >> 16), (byte)(n >> 24) };
        }

        public static byte[] ToLittleEndian(long n)
        {
            return new byte[] { (byte)n, (byte)(n >> 8), (byte)(n >> 16), (byte)(n >> 24), (byte)(n >> 32), (byte)(n >> 40), (byte)(n >> 48), (byte)(n >> 56) };
        }
    }
}
