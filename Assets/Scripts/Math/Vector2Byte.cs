using System;
using UnityEngine;

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

        public static explicit operator Vector2(Vector2Byte v) => new(v.x, v.y);
        public static explicit operator Vector2Byte(Vector2 v) => new((byte)Mathf.RoundToInt(v.x), (byte)Mathf.RoundToInt(v.y));
    }
}
