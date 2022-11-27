namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public static class SyntaxErrors
    {
        public static string UnknownInstruction(Token token) => 
            $"Unknown instruction \"{token.Content}\"";

        public static string AbsoluteIndexedNotSupported(Token token) =>
            $"The instruction \"{token.Content}\" does not support absolute indexed addressing";

        public static string RegisterNotXY(Token token) => 
            $"Invalid register, expected X or Y";

        public static string IndexedIndirectNotZeroPage(Token token) =>
            $"The address in indexed indirect addressing must be a zero-page address";

        public static string IndirectIndexedNotZeroPage(Token token) =>
            $"The address in indirect indexed addressing must be a zero-page address";
    }
}