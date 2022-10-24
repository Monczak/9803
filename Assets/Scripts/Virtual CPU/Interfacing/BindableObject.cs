using System;

namespace NineEightOhThree.VirtualCPU.Interfacing
{
    [Serializable]
    public class BindableObject<T> : Bindable where T : ISerializableBindableObject
    {
        public new T value;

        public new void SetValueFromBytes(byte[] bytes)
        {
            value = (T)value.FromBytes(bytes);
            dirty = true;
        }
        
        public new byte[] GetBytes()
        {
            return value.ToBytes();
        }

        public new bool SetValueFromString(string str)
        {
            try
            {
                value = (T)value.Deserialize(str);
                return true;
            }
            catch (InvalidOperationException)
            {
                // Debug.LogError($"{e.Message}\n{e.StackTrace}");
                return false;
            }
        }

        public new int Bytes => value.Bytes;
        
        public new void OnBeforeSerialize()
        {
            if (value == null)
            {
                serializedValue = "n";
                return;
            }

            serializedValue = value.Serialize();
        }

        public new void OnAfterDeserialize()
        {
            if (type == BindableType.Null)
            {
                value = default;
                return;
            }

            value = (T)value.Deserialize(serializedValue);
        }
    }
}