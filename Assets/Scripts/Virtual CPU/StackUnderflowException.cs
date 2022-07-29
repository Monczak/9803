using System;

namespace NineEightOhThree.VirtualCPU
{

	[Serializable]
	public class StackUnderflowException : Exception
	{
		public StackUnderflowException() { }
		public StackUnderflowException(string message) : base(message) { }
		public StackUnderflowException(string message, Exception inner) : base(message, inner) { }
		protected StackUnderflowException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
