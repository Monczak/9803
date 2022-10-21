using System;
using System.Runtime.Serialization;

namespace NineEightOhThree.Inventory
{
    [Serializable]
    public class InvalidItemException : Exception
    {
        public byte Id { get; }
        
        public InvalidItemException()
        {
        }

        public InvalidItemException(string message) : base(message)
        {
        }

        public InvalidItemException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidItemException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public InvalidItemException(byte id)
        {
            Id = id;
        }
    }
}