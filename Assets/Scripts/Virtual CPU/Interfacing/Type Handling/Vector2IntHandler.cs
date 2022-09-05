using System;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU.Interfacing.TypeHandling
{
    public class Vector2IntHandler : IBindableTypeHandler
    {
        public int Bytes => 2;

        public object Deserialize(string serializedValue) => new Vector2Int(int.Parse(serializedValue.Split("|")[0]), int.Parse(serializedValue.Split("|")[1]));

        public object Parse(string str) => new Vector2Int(int.Parse(str.Split(" ")[0]), int.Parse(str.Split(" ")[1]));

        public string Serialize(object value) => $"{((Vector2Int)value).x}|{((Vector2Int)value).y}";

        public byte[] ToBytes(object value) => throw new NotImplementedException();
        public object FromBytes(byte[] bytes) => throw new NotImplementedException();
    }
}