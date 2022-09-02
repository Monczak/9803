using System;
using System.Collections.Generic;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU.Interfacing
{

    [Serializable]
    public class Bindable : ScriptableObject
    {
        [SerializeField] protected object value;
        [SerializeField] protected ushort address;

        [SerializeField] protected Type type;

        public T GetValue<T>()
        {
            if (value == null)
                return default;
            return (T)value;
        }
        public void SetValue<T>(T value)
        {
            type = typeof(T);
            this.value = value;
        }

        private T ParseValue<T>(string str)
        {
            return (T)ParseValue(str, typeof(T));
        }

        private object ParseValue(string str, Type type)
        {
            try
            {
                if (!Parsers.ContainsKey(type))
                    throw new InvalidOperationException($"No parser exists for type {type.Name}");

                this.type = type;

                (ParseFunction parser, _) = Parsers[type];
                return parser(str);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        public bool SetValueFromString(string str, Type type)
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

        public delegate object ParseFunction(string str);
        public static readonly Dictionary<Type, (ParseFunction, int)> Parsers = new()   // (ParseFunction, int) -> (parser, number of bytes in result)
        {
            { typeof(byte), (str => str.StartsWith("0x") ? byte.Parse(str[2..], System.Globalization.NumberStyles.HexNumber) : byte.Parse(str), 1) },
            { typeof(int), (str => int.Parse(str), 2) },
            { typeof(long), (str => long.Parse(str), 4) },
            { typeof(bool), (str => str != "0" && str.Trim() != "" && str.Trim().ToLower() != "false", 1) },
            { typeof(Vector2), (str => new Vector2(int.Parse(str.Split(" ")[0]), int.Parse(str.Split(" ")[1])), 2) },
        };
    }
}
