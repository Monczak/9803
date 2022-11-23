namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{

    [System.Serializable]
    public class SyntaxErrorException : System.Exception, IAssemblerException
    {
        public char Char { get; init; }
        public int Line { get; init; }
        public int Column { get; init; }

        public SyntaxErrorException() { }
        public SyntaxErrorException(string message) : base(message) { }
        public SyntaxErrorException(string message, System.Exception inner) : base(message, inner) { }
        protected SyntaxErrorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public SyntaxErrorException(string message, char c, int line) : base($"{message} '{c}' (line {line})")
        {
            Char = c;
            Line = line;
        }
    }
}