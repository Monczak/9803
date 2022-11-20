using NineEightOhThree.VirtualCPU.Interfacing.TypeHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU.Interfacing
{
    [Serializable]
    public class Bindable : ScriptableObject, ISerializationCallbackReceiver
    {
        public object value;
        public ushort[] addresses;

        public BindableType type = BindableType.Null;
        public string objectTypeName;
        public string fieldName;
        public string parentObjectName;
        public string parentClassName;

        [SerializeField] protected string serializedValue;

        public bool enabled;
        public bool dirty;

        private bool initializeLazily;
        public bool IsPointer { get; private set; }

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

        public void ForceUpdate()
        {
            ForceSerialize();
            dirty = true;
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
                {
                    value = ((ISerializableBindableObject)value).Deserialize(str);
                    IsPointer = ((ISerializableBindableObject)value).IsPointer;
                }
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
        public int AddressCount => IsPointer ? 1 : Bytes;

        public void OnBeforeSerialize()
        {
            ForceSerialize();
        }

        public void ForceSerialize()
        {
            if (value == null)
            {
                if (serializedValue != "" && serializedValue != "n")
                {
                    CreateNewValue();
                    SetValueFromString(serializedValue);
                    return;
                }
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
            {
                initializeLazily = true;
            }
            else
                value = Handlers[type].Deserialize(serializedValue);
        }

        public void DeserializeIfHasData()
        {
            if (HasData)
                SetValueFromString(serializedValue);
        }

        public bool HasData => serializedValue is not null && serializedValue != "n";

        public void SetValueNullIfSerializedNull()
        {
            if (serializedValue == "n")
                value = null;
        }

        public void CreateNewValue()
        {
            Type csType = Type.GetType(objectTypeName);
            if (csType is null) return;
            
            if (csType.IsSubclassOf(typeof(ScriptableObject)))
                value ??= CreateInstance(csType);
            else
                value ??= Activator.CreateInstance(csType);
        }

        private void Awake()
        {
            if (value is null && serializedValue is not null && initializeLazily)
            {
                Type type = Type.GetType(objectTypeName);
                ISerializableBindableObject obj;
                if (type.IsSubclassOf(typeof(ScriptableObject)))
                {
                    obj = (ISerializableBindableObject)CreateInstance(type);
                }
                else
                {
                    obj = (ISerializableBindableObject)Activator.CreateInstance(type);
                }
                value = obj.Deserialize(serializedValue);
            }
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

        public override string ToString()
        {
            return $"{parentObjectName} {parentClassName}.{fieldName} = {value ?? serializedValue} ({string.Join(", ", addresses.Select(a => a.ToString("X4")))})";
        }
    }
}
