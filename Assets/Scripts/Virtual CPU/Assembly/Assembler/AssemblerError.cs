namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public struct AssemblerError
    {
        public enum ErrorType
        {
            Lexical,
            Syntax,
            Internal
        }
        
        public string Message { get; }
        public Token? Token { get; private set; }
        public char? Char { get; }
        public int? Line { get; }
        
        public ErrorType Type { get; }

        public AssemblerError(ErrorType type, string message, Token? token)
        {
            Type = type;
            Message = message;
            Token = token;
            Char = null;
            Line = null;
        }
        
        public AssemblerError(ErrorType type, string message, char c, int line)
        {
            Type = type;
            Message = message;
            Token = null;
            Char = c;
            Line = line;
        }

        public AssemblerError WithToken(Token token)
        {
            Token = token;
            return this;
        }
    }
}