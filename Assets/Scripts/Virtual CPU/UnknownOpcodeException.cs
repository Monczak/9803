using System;

namespace NineEightOhThree.VirtualCPU
{

	[Serializable]
	public class UnknownOpcodeException : Exception
	{
		public byte Opcode { get; private set; }

		public UnknownOpcodeException() { }
		public UnknownOpcodeException(string message) : base(message) { }
		public UnknownOpcodeException(string message, Exception inner) : base(message, inner) { }
		protected UnknownOpcodeException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

		public UnknownOpcodeException(byte opcode)
		{
			Opcode = opcode;
		}
	}
}
