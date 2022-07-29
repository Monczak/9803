using System;

namespace NineEightOhThree.VirtualCPU
{

	[Serializable]
	public class UnknownInstructionException : Exception
	{
		public string Mnemonic { get; private set; }

		public UnknownInstructionException() { }
		public UnknownInstructionException(string message, Exception inner) : base(message, inner) { }
		protected UnknownInstructionException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

		public UnknownInstructionException(string mnemonic) : base(mnemonic)
		{
			Mnemonic = mnemonic;
		}
	}
}
