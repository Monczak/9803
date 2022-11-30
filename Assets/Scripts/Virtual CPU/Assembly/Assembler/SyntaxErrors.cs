using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public static class SyntaxErrors
    {
        private static readonly Dictionary<AddressingMode, string> AddressingModeNames = new()
        {
            { AddressingMode.Absolute, "absolute" },
            { AddressingMode.AbsoluteX, "absolute indexed" },
            { AddressingMode.AbsoluteY, "absolute indexed" },
            { AddressingMode.Accumulator, "accumulator" },
            { AddressingMode.Immediate, "immediate" },
            { AddressingMode.Implied, "implied" },
            { AddressingMode.IndexedIndirect, "indexed indirect" },
            { AddressingMode.Indirect, "indirect" },
            { AddressingMode.IndirectIndexed, "indirect indexed" },
            { AddressingMode.ZeroPage, "zero page" },
            { AddressingMode.ZeroPageX, "zero page indexed" },
            { AddressingMode.ZeroPageY, "zero page indexed" },
            { AddressingMode.Relative, "relative" },
        };

        public static string UnknownInstruction(Token token) =>
            $"Unknown instruction \"{token.Content}\"";

        public static string AddressingModeNotSupported(Token token, AddressingMode addressingMode) =>
            $"The instruction \"{token.Content}\" does not support {AddressingModeNames[addressingMode]} addressing";

        public static string RegisterNotXY(Token token) =>
            $"Invalid register, expected X or Y";

        public static string IndexedIndirectNotZeroPage(Token token) =>
            $"The address in indexed indirect addressing must be a zero-page address";

        public static string IndirectIndexedNotZeroPage(Token token) =>
            $"The address in indirect indexed addressing must be a zero-page address";

        public static string ExpectedGot(TokenType expectedTypeMask, TokenType gotType) =>
            (expectedTypeMask, gotType) switch
            {
                (TokenType.Newline or TokenType.EndOfFile, _) => $"Expected end of statement, got {gotType.ToString()}",
                (_, TokenType.Newline or TokenType.EndOfFile) =>
                    $"Unterminated statement, expected {expectedTypeMask.ToString()}",
                _ => $"Expected {expectedTypeMask.ToString()}, got {gotType.ToString()}"
            };
    }
}