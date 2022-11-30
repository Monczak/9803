namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public static class LexicalErrors
    {
        public static AssemblerError ExpectedGot(char c, int line, TokenType expectedTypeMask, TokenType gotType) =>
            new(AssemblerError.ErrorType.Lexical, $"Expected {expectedTypeMask.ToString()}, got {gotType.ToString()}",
                c, line);

        public static AssemblerError UnexpectedCharacter(char c, int line) =>
            new(AssemblerError.ErrorType.Lexical, $"Unexpected character '{c}'", c, line);
    }
}