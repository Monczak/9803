using System;
using System.Runtime.Serialization;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    [Serializable]
    public class SyntaxErrorException : Exception
    {
        public Token Token { get; init; }
        public int Line { get; init; }
        public int Column { get; init; }

        public SyntaxErrorException()
        {
        }

        public SyntaxErrorException(string message) : base(message)
        {
        }

        public SyntaxErrorException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SyntaxErrorException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
        
        public SyntaxErrorException(string message, Token token) : base($"{message} ({token}) (line {token.Line})")
        {
            Token = token;
            Line = token.Line;
        }
    }
}