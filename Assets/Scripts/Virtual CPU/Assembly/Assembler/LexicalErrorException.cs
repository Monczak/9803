namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{

    [System.Serializable]
    public class LexicalErrorException : System.Exception, IAssemblerException
    {
        public char Char { get; init; }
        public int Line { get; init; }
        public int Column { get; init; }

        public LexicalErrorException() { }
        public LexicalErrorException(string message) : base(message) { }
        public LexicalErrorException(string message, System.Exception inner) : base(message, inner) { }
        protected LexicalErrorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public LexicalErrorException(string message, char c, int line) : base($"{message} '{c}' (line {line})")
        {
            Char = c;
            Line = line;
        }
    }
}