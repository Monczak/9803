using System;

namespace NineEightOhThree.Math
{
    [Serializable]
    public struct Vector2Byte
    {
        public byte x, y;

        public Vector2Byte(byte x, byte y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2Byte(int x, int y)
        {
            this.x = (byte)x;
            this.y = (byte)y;
        }

        public override string ToString() => $"{x} {y}";
    }
}
