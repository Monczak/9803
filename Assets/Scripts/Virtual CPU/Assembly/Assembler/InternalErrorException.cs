using System;
using System.Runtime.Serialization;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    [Serializable]
    public class InternalErrorException : Exception, IAssemblerException
    {
        public InternalErrorException()
        {
        }

        public InternalErrorException(string message) : base(message)
        {
        }

        public InternalErrorException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InternalErrorException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}