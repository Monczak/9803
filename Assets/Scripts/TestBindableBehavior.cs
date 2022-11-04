using System;
using NineEightOhThree.VirtualCPU.Interfacing;
using UnityEngine;

namespace NineEightOhThree
{
    public class TestBindableBehavior : MemoryBindableBehavior
    {
        [BindableType(BindableType.Byte), HideInInspector]
        public Bindable testBindable;

        [BindableType(typeof(ByteWrapper)), HideInInspector]
        public Bindable testObjectBindable;
    }

    [Serializable]
    public class ByteWrapper : ISerializableBindableObject
    {
        public byte b;
        public byte[] ToBytes()
        {
            return new[] { b };
        }

        public object FromBytes(byte[] bytes)
        {
            return new ByteWrapper { b = bytes[0] };
        }

        public string Serialize()
        {
            return b.ToString();
        }

        public object Deserialize(string str)
        {
            return new ByteWrapper { b = byte.Parse(str) };
        }

        public int Bytes => 1;
        public bool IsPointer => false;
    }
}
