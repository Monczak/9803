namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public struct ParserError
    {
        public string Message { get; }
        public Token? Token { get; }

        public ParserError(string message, Token? token)
        {
            Message = message;
            Token = token;
        }
    }
}