namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public static class LexicalErrors
    {
        public static AssemblerError ExpectedGot(char c, int line, int column, int charIndex, int length, TokenType expectedTypeMask, TokenType gotType) =>
            new(AssemblerError.ErrorType.Lexical, $"Expected {expectedTypeMask.ToString()}, got {gotType.ToString()}",
                c, line, column, charIndex, length);

        public static AssemblerError UnexpectedCharacter(char c, int line, int column, int charIndex, int length) =>
            new(AssemblerError.ErrorType.Lexical, $"Unexpected character '{c}'", c, line, column, charIndex, length);
        
        public static AssemblerError InvalidNumber(char c, int line, int column, int charIndex, int length) =>
            new(AssemblerError.ErrorType.Lexical, $"Invalid number (should be in range 0 - 65535)", c, line, column, charIndex, length);
    }
}