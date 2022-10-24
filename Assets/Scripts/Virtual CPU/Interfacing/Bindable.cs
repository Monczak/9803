﻿using NineEightOhThree.VirtualCPU.Interfacing.TypeHandling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU.Interfacing
{
    [Serializable]
    public class Bindable : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeReference] public object value;
        public ushort[] addresses;

        public BindableType type = BindableType.Null;
        public string objectTypeName;
        public string fieldName;

        [SerializeField] protected string serializedValue;

        public bool enabled;
        public bool dirty;

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
            if (type == BindableType.Object)
                value = ((ISerializableBindableObject)value).FromBytes(bytes);
            else
                value = Handlers[type].FromBytes(bytes);
            dirty = true;
        }
        public byte[] GetBytes()
        {
            return type == BindableType.Object ? ((ISerializableBindableObject)value).ToBytes() : Handlers[type].ToBytes(value);
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
                if (type == BindableType.Object)
                    value = ((ISerializableBindableObject)value)?.Deserialize(str);
                else
                    value = ParseValue(str, type);
                return true;
            }
            catch (InvalidOperationException)
            {
                // Debug.LogError($"{e.Message}\n{e.StackTrace}");
                return false;
            }
        }

        public int Bytes => type == BindableType.Object ? ((ISerializableBindableObject)value).Bytes : Handlers[type].Bytes;

        public void OnBeforeSerialize()
        {
            if (value == null)
            {
                serializedValue = "n";
                return;
            }

            if (type == BindableType.Object)
                serializedValue = ((ISerializableBindableObject)value).Serialize();
            else
                serializedValue = Handlers[type].Serialize(value);
        }

        public void OnAfterDeserialize()
        {
            if (type == BindableType.Null)
            {
                value = null;
                return;
            }

            if (type == BindableType.Object)
                value = ((ISerializableBindableObject)value).Deserialize(serializedValue);
            else
                value = Handlers[type].Deserialize(serializedValue);
        }

        public void DeserializeIfHasData()
        {
            if (serializedValue is not null)
                SetValueFromString(serializedValue);
        }

        public static readonly Dictionary<BindableType, IBindableTypeHandler> Handlers = new()
        {
            { BindableType.Byte, new ByteHandler() },
            { BindableType.Ushort, new UshortHandler() },
            { BindableType.Int, new IntHandler() },
            { BindableType.Long, new LongHandler() },
            { BindableType.Bool, new BoolHandler() },
            { BindableType.Vector2, new Vector2Handler() },
            { BindableType.Vector2Int, new Vector2IntHandler() },
            { BindableType.Vector2Byte, new Vector2ByteHandler() }
        };
    }
}
