namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public struct AssemblerError
    {
        public enum ErrorType
        {
            Lexical,
            Syntax
        }
        
        public string Message { get; }
        public Token? Token { get; }
        public char? Char { get; }
        public int? Line { get; }

        public AssemblerError(ErrorType type, string message, Token? token)
        {
            Message = message;
            Token = token;
            Char = null;
            Line = null;
        }
        
        public AssemblerError(ErrorType type, string message, char c, int line)
        {
            Message = message;
            Token = null;
            Char = c;
            Line = line;
        }
    }
}