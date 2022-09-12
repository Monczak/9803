using NineEightOhThree.VirtualCPU.Utilities;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU.Interfacing.TypeHandling
{
    public class Vector2IntHandler : IBindableTypeHandler
    {
        public int Bytes => 8;

        public object Deserialize(string serializedValue) => new Vector2Int(int.Parse(serializedValue.Split("|")[0]), int.Parse(serializedValue.Split("|")[1]));

        public object Parse(string str) => new Vector2Int(int.Parse(str.Split(" ")[0]), int.Parse(str.Split(" ")[1]));

        public string Serialize(object value) => $"{((Vector2Int)value).x}|{((Vector2Int)value).y}";

        public byte[] ToBytes(object value)
        {
            Vector2Int v = (Vector2Int)value;
            byte[] xBytes = BitUtils.ToLittleEndian(v.x);
            byte[] yBytes = BitUtils.ToLittleEndian(v.y);
            return new byte[] { xBytes[0], xBytes[1], xBytes[2], xBytes[3], yBytes[0], yBytes[1], yBytes[2], yBytes[3] };
        }
        public object FromBytes(byte[] bytes) => new Vector2Int((int)BitUtils.FromLittleEndian<int>(bytes[0..3]), (int)BitUtils.FromLittleEndian<int>(bytes[4..7]));
    }
}