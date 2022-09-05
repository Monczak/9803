using System;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU.Interfacing.TypeHandling
{
    public class Vector2Handler : IBindableTypeHandler
    {
        public int Bytes => 2;

        public object Deserialize(string serializedValue) => new Vector2(float.Parse(serializedValue.Split("|")[0]), float.Parse(serializedValue.Split("|")[1]));

        public object Parse(string str) => new Vector2(int.Parse(str.Split(" ")[0]), int.Parse(str.Split(" ")[1]));

        public string Serialize(object value) => $"{((Vector2)value).x}|{((Vector2)value).y}";

        public byte[] ToBytes(object value) => throw new NotImplementedException();
        public object FromBytes(byte[] bytes) => throw new NotImplementedException();
    }
}