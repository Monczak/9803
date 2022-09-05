using NineEightOhThree.VirtualCPU.Interfacing.TypeHandling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU.Interfacing
{

    [Serializable]
    public class Bindable : ScriptableObject, ISerializationCallbackReceiver
    {
        public object value;
        public ushort address;

        public BindableType type = BindableType.Null;
        public string fieldName;

        [SerializeField]
        private string serializedValue;

        public T GetValue<T>()
        {
            if (value == null)
                return default;
            return (T)value;
        }
        public void SetValue<T>(T value)
        {
            this.value = value;
        }
        public void SetValueFromBytes(byte[] bytes)
        {
            value = Handlers[type].FromBytes(bytes);
        }
        public byte[] GetBytes()
        {
            return Handlers[type].ToBytes(value);
        }

        private T ParseValue<T>(string str)
        {
            return (T)ParseValue(str, type);
        }

        private object ParseValue(string str, BindableType type)
        {
            try
            {
                if (!Handlers.ContainsKey(type))
                    throw new InvalidOperationException($"No handler exists for type {type}");

                this.type = type;

                return Handlers[type].Parse(str);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        public bool SetValueFromString(string str)
        {
            try
            {
                value = ParseValue(str, type);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public int Bytes => Handlers[type].Bytes;

        public void OnBeforeSerialize()
        {
            if (value == null)
            {
                serializedValue = "n";
                return;
            }

            serializedValue = Handlers[type].Serialize(value);
        }

        public void OnAfterDeserialize()
        {
            if (type == BindableType.Null)
            {
                value = null;
                return;
            }

            value = Handlers[type].Deserialize(serializedValue);
        }

        public static readonly Dictionary<BindableType, IBindableTypeHandler> Handlers = new()
        {
            { BindableType.Byte, new ByteHandler() },
            { BindableType.Int, new IntHandler() },
            { BindableType.Long, new LongHandler() },
            { BindableType.Bool, new BoolHandler() },
            { BindableType.Vector2, new Vector2Handler() },
        };
    }
}
