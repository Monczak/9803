namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{

    [System.Serializable]
    public class SyntaxError : System.Exception
    {
        public int Line { get; init; }
        public int Column { get; init; }

        public SyntaxError() { }
        public SyntaxError(string message) : base(message) { }
        public SyntaxError(string message, System.Exception inner) : base(message, inner) { }
        protected SyntaxError(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public SyntaxError(string message, int line) : base(message)
        {
            Line = line;
        }
    }
}