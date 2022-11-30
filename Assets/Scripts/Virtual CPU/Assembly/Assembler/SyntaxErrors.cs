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

        public static AssemblerError UnknownInstruction(Token token) =>
            new(AssemblerError.ErrorType.Syntax, $"Unknown instruction \"{token.Content}\"", token);

        public static AssemblerError AddressingModeNotSupported(Token token, AddressingMode addressingMode) =>
            new(AssemblerError.ErrorType.Syntax ,$"The instruction \"{token.Content}\" does not support {AddressingModeNames[addressingMode]} addressing", token);

        public static AssemblerError RegisterNotXY(Token token) =>
            new(AssemblerError.ErrorType.Syntax, $"Invalid register, expected X or Y", token);

        public static AssemblerError IndexedIndirectNotZeroPage(Token token) =>
            new(AssemblerError.ErrorType.Syntax, $"The address in indexed indirect addressing must be a zero-page address", token);

        public static AssemblerError IndirectIndexedNotZeroPage(Token token) =>
            new(AssemblerError.ErrorType.Syntax, $"The address in indirect indexed addressing must be a zero-page address", token);

        public static AssemblerError ExpectedGot(Token token, TokenType expectedTypeMask, TokenType gotType) =>
            new(AssemblerError.ErrorType.Syntax, (expectedTypeMask, gotType) switch
            {
                (TokenType.Newline or TokenType.EndOfFile, _) => $"Expected end of statement, got {gotType.ToString()}",
                (_, TokenType.Newline or TokenType.EndOfFile) =>
                    $"Unterminated statement, expected {expectedTypeMask.ToString()}",
                _ => $"Expected {expectedTypeMask.ToString()}, got {gotType.ToString()}"
            }, token);
    }
}