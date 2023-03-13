using System.Collections.Generic;
using System.Linq;
using NineEightOhThree.Utilities;
using NineEightOhThree.VirtualCPU.Assembly.Assembler.Directives;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public static class SyntaxErrors
    {
        private static readonly Dictionary<AddressingMode, string> AddressingModeNames = new()
        {
            { AddressingMode.Absolute, "absolute" },
            { AddressingMode.AbsoluteX, "absolute X-indexed" },
            { AddressingMode.AbsoluteY, "absolute Y-indexed" },
            { AddressingMode.Accumulator, "accumulator" },
            { AddressingMode.Immediate, "immediate" },
            { AddressingMode.Implied, "implied" },
            { AddressingMode.IndexedIndirect, "indexed indirect" },
            { AddressingMode.Indirect, "indirect" },
            { AddressingMode.IndirectIndexed, "indirect indexed" },
            { AddressingMode.ZeroPage, "zero page" },
            { AddressingMode.ZeroPageX, "zero page X-indexed" },
            { AddressingMode.ZeroPageY, "zero page Y-indexed" },
            { AddressingMode.Relative, "relative" },
        };

        public static AssemblerError UnknownInstruction(Token token) =>
            new(AssemblerError.ErrorType.Syntax, $"Unknown instruction \"{token.Content}\"", token);

        public static AssemblerError AddressingModeNotSupported(Token token, AddressingMode addressingMode) =>
            new(AssemblerError.ErrorType.Syntax ,$"The instruction \"{token.Content}\" does not support {string.Join("/", EnumUtils.DeconstructFlags(addressingMode).Select(m => AddressingModeNames[m]))} addressing", token);

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
            }, (expectedTypeMask, gotType) switch
            {
                (_, TokenType.Newline or TokenType.EndOfFile) => token.Previous,
                _ => token
            });

        public static AssemblerError Expected(Token token, TokenType expectedTypeMask) =>
            new(AssemblerError.ErrorType.Syntax, expectedTypeMask switch
            {
                TokenType.Newline or TokenType.EndOfFile => $"Expected end of statement",
                _ => $"Expected {expectedTypeMask.ToString()}"
            }, token);
        
        public static AssemblerError UnknownDirective(Token token) =>
            new(AssemblerError.ErrorType.Syntax, $"Unknown directive \"{token.Content[1..]}\"", token);

        public static AssemblerError OperandNotByte(Token token) =>
            new(AssemblerError.ErrorType.Syntax, $"Expected number in range 0-255", token);

        public static AssemblerError UnexpectedToken(Token token) =>
            new(AssemblerError.ErrorType.Syntax, $"Unexpected token \"{token.Content}\"", token);

        public static AssemblerError LabelAlreadyDeclared(Token token) =>
            new(AssemblerError.ErrorType.Syntax, $"Label {token.Content[..^1]} is already defined", token);

        public static AssemblerError WrongArgumentCount(Token token, int count) =>
            new(AssemblerError.ErrorType.Syntax,
                $"Wrong argument count, expected {(count == -1 ? "at least 1 argument" : $"{count} {(count == 1 ? "argument" : "arguments")}")}", token);

        public static AssemblerError UseOfUndeclaredLabel(Token token, Label label) =>
            new(AssemblerError.ErrorType.Syntax, $"Use of undeclared label \"{label.Name}\"", token);

        public static AssemblerError OverlappingCode(Token token, ushort address) =>
            new(AssemblerError.ErrorType.Syntax, $"Overlapping code (at address {address:X4})", token);
        
        public static AssemblerError DuplicateSingleDirective(Token token) =>
            new(AssemblerError.ErrorType.Syntax, $"The {token.Content} directive is already present", token);

        public static AssemblerError InvalidIncludedResourceLocation(Token token, string resourceLocation) =>
            new(AssemblerError.ErrorType.Syntax, $"The specified resource location ({resourceLocation}) is invalid", token);

        public static AssemblerError CircularDependency(Token token, string resourceLocation) =>
            new(AssemblerError.ErrorType.Syntax, $"Circular dependency - cannot include {resourceLocation} here", token);
    }
}