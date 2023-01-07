using System;
using System.Collections.Generic;
using System.Linq;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements
{
    public abstract class InstructionStatement : FinalStatement
    {
        public CPUInstruction CPUInstruction { get; protected set; }
        public AddressingMode AddressingMode { get; protected set; }
        public CPUInstructionMetadata Metadata => CPUInstruction.Metadata[AddressingMode];

        private Token instructionToken;
        
        protected InstructionStatement(List<Token> tokens) : base(tokens)
        {
            
        }

        protected IEnumerable<(CPUInstruction instruction, AddressingMode addressingMode, CPUInstructionMetadata metadata)> InstructionCandidates
        {
            get;
            private set;
        }
        
        protected OperationResult FindCandidates(Token token, AddressingMode modeFlags)
        {
            instructionToken = token;
            
            if (!CPUInstructionRegistry.TryGetInstructions(token.Content, out var candidates))
                return OperationResult.Error(SyntaxErrors.UnknownInstruction(token));
            
            instructionToken.SetMetaType(TokenMetaType.Instruction);
            
            InstructionCandidates = candidates.Where(info => (info.addressingMode & modeFlags) != 0);
            
            if (!InstructionCandidates.Any())
                return OperationResult.Error(
                    SyntaxErrors.AddressingModeNotSupported(token, AddressingMode));
            
            return OperationResult.Success();
        }

        protected OperationResult FindInstruction(Token token)
        {
            OperationResult candidateResult = FindCandidates(token, AddressingMode);
            if (candidateResult.Failed)
                return candidateResult;
            
            return MatchInstructionFromFound(AddressingMode);
        }

        protected OperationResult MatchInstructionFromFound(AddressingMode addressingMode)
        {
            if (!IsSupported(addressingMode)) return OperationResult.Error(SyntaxErrors.AddressingModeNotSupported(instructionToken, addressingMode));
            CPUInstruction = InstructionCandidates.First(info => info.addressingMode == addressingMode).instruction;
            AddressingMode = addressingMode;
            return OperationResult.Success();
        }

        protected bool IsSupported(AddressingMode addressingMode) =>
            InstructionCandidates.Any(info => info.addressingMode == addressingMode);
    }

    public abstract class InstructionStatementOperand : InstructionStatement
    {
        public Operand Operand { get; protected set; }
        
        protected InstructionStatementOperand(List<Token> tokens) : base(tokens)
        {
        }

        protected OperationResult SetOperand(Token token)
        {
            Operand = token.Type switch
            {
                TokenType.Number => new Operand(token, (ushort)token.Literal),
                TokenType.Identifier => new Operand(token, token.Content),
                _ => Operand
            };
            return OperationResult.Success();
        }
    }

    public sealed class InstructionStatementImplied : InstructionStatement
    {
        public InstructionStatementImplied(List<Token> tokens) : base(tokens)
        {
            AddressingMode = AddressingMode.Implied;
        }

        protected internal override List<(NodePattern pattern, TokenHandler handler)> Pattern => new()
        {
            (NodePattern.Single(TokenType.Identifier), FindInstruction),
        };

        protected override AbstractStatement Construct(List<Token> tokens) =>
            new InstructionStatementImplied(tokens);
    }

    public sealed class InstructionStatementAccumulator : InstructionStatement
    {
        public InstructionStatementAccumulator(List<Token> tokens) : base(tokens)
        {
            AddressingMode = AddressingMode.Accumulator;
        }

        protected internal override List<(NodePattern pattern, TokenHandler handler)> Pattern => new()
        {
            (NodePattern.Single(TokenType.Identifier), FindInstruction),
            (NodePattern.Single(TokenType.RegisterA), null)
        };

        protected override AbstractStatement Construct(List<Token> tokens) =>
            new InstructionStatementAccumulator(tokens);
    }

    public sealed class InstructionStatementAbsoluteRelative : InstructionStatementOperand
    {
        public InstructionStatementAbsoluteRelative(List<Token> tokens) : base(tokens)
        {
            
        }

        protected internal override List<(NodePattern pattern, TokenHandler handler)> Pattern => new()
        {
            (NodePattern.Single(TokenType.Identifier), token => FindCandidates(token, AddressingMode.Absolute | AddressingMode.ZeroPage | AddressingMode.Relative)),
            (NodePattern.Single(TokenType.Number | TokenType.Identifier), token =>
            {
                SetOperand(token);

                // Assume that if the first instruction in the candidates is a branch, all other are as well
                // (this enumerable contains multiple types of one instruction)
                bool isBranch = InstructionCandidates.First().instruction.IsBranch;

                if (isBranch)
                {
                    return MatchInstructionFromFound(AddressingMode.Relative);
                }
                if (Operand.IsDefined && Operand.Number < 256)
                {
                    return MatchInstructionFromFound(IsSupported(AddressingMode.ZeroPage)
                        ? AddressingMode.ZeroPage
                        : AddressingMode.Absolute);
                }

                return MatchInstructionFromFound(AddressingMode.Absolute);
            })
        };
        
        protected override AbstractStatement Construct(List<Token> tokens) => new InstructionStatementAbsoluteRelative(tokens);
    }

    public sealed class InstructionStatementImmediateOp : InstructionStatementOperand
    {
        public InstructionStatementImmediateOp(List<Token> tokens) : base(tokens)
        {
            AddressingMode = AddressingMode.Immediate;
        }

        protected internal override List<(NodePattern pattern, TokenHandler handler)> Pattern => new()
        {
            (NodePattern.Single(TokenType.Identifier), FindInstruction),
            (NodePattern.Single(TokenType.ImmediateOp), null),
            (NodePattern.Single(TokenType.Number | TokenType.Identifier), SetOperand)
        };

        protected override AbstractStatement Construct(List<Token> tokens) => new InstructionStatementImmediateOp(tokens);
    }

    public sealed class InstructionStatementAbsoluteIndexed : InstructionStatementOperand
    {
        public InstructionStatementAbsoluteIndexed(List<Token> tokens) : base(tokens)
        {
            
        }

        protected internal override List<(NodePattern pattern, TokenHandler handler)> Pattern => new()
        {
            (NodePattern.Single(TokenType.Identifier), token => FindCandidates(token, AddressingMode.AbsoluteX | AddressingMode.AbsoluteY | AddressingMode.ZeroPageX | AddressingMode.ZeroPageY)),
            (NodePattern.Single(TokenType.Number | TokenType.Identifier), SetOperand),
            (NodePattern.Single(TokenType.Comma), null),
            (NodePattern.Single(TokenType.RegisterX | TokenType.RegisterY), token =>
            {
                switch (Operand.IsDefined, Operand.IsDefined && Operand.Number < 256, token.Type)
                {
                    case (_, false, TokenType.RegisterX): return MatchInstructionFromFound(AddressingMode.AbsoluteX);
                    case (_, false, TokenType.RegisterY): return MatchInstructionFromFound(AddressingMode.AbsoluteY);
                    case (true, true, TokenType.RegisterX): return MatchInstructionFromFound(AddressingMode.ZeroPageX);
                    case (true, true, TokenType.RegisterY): return MatchInstructionFromFound(AddressingMode.ZeroPageY);
                    default: return OperationResult.Error(SyntaxErrors.RegisterNotXY(token));
                }
            })
        };

        protected override AbstractStatement Construct(List<Token> tokens) =>
            new InstructionStatementAbsoluteIndexed(tokens);
    }

    public sealed class InstructionStatementIndirect : InstructionStatementOperand
    {
        public InstructionStatementIndirect(List<Token> tokens) : base(tokens)
        {
            AddressingMode = AddressingMode.Indirect;
        }

        protected internal override List<(NodePattern pattern, TokenHandler handler)> Pattern => new()
        {
            (NodePattern.Single(TokenType.Identifier), FindInstruction),
            (NodePattern.Single(TokenType.LeftParen), null),
            (NodePattern.Single(TokenType.Number | TokenType.Identifier), SetOperand),
            (NodePattern.Single(TokenType.RightParen), null)
        };

        protected override AbstractStatement Construct(List<Token> tokens) =>
            new InstructionStatementIndirect(tokens);
    }

    public sealed class InstructionStatementIndexedIndirect : InstructionStatementOperand
    {
        public InstructionStatementIndexedIndirect(List<Token> tokens) : base(tokens)
        {
            AddressingMode = AddressingMode.IndexedIndirect;
        }

        protected internal override List<(NodePattern pattern, TokenHandler handler)> Pattern => new()
        {
            (NodePattern.Single(TokenType.Identifier), FindInstruction),
            (NodePattern.Single(TokenType.LeftParen), null),
            (NodePattern.Single(TokenType.Number | TokenType.Identifier), token =>
            {
                SetOperand(token);
                if (Operand.IsDefined)  // If the operand is undefined (label), check if valid later
                {
                    if (Operand.Number > 255)
                        return OperationResult.Error(
                            SyntaxErrors.IndexedIndirectNotZeroPage(token));
                }
                return OperationResult.Success();
            }),
            (NodePattern.Single(TokenType.Comma), null),
            (NodePattern.Single(TokenType.RegisterX), null),
            (NodePattern.Single(TokenType.RightParen), null),
        };

        protected override AbstractStatement Construct(List<Token> tokens) =>
            new InstructionStatementIndexedIndirect(tokens);
    }

    public sealed class InstructionStatementIndirectIndexed : InstructionStatementOperand
    {
        public InstructionStatementIndirectIndexed(List<Token> tokens) : base(tokens)
        {
            AddressingMode = AddressingMode.IndirectIndexed;
        }

        protected internal override List<(NodePattern pattern, TokenHandler handler)> Pattern => new()
        {
            (NodePattern.Single(TokenType.Identifier), FindInstruction),
            (NodePattern.Single(TokenType.LeftParen), null),
            (NodePattern.Single(TokenType.Number | TokenType.Identifier), token =>
            {
                SetOperand(token);
                if (Operand.IsDefined)  // If the operand is undefined (label), check if valid later
                {
                    if (Operand.Number > 255)
                        return OperationResult.Error(
                            SyntaxErrors.IndirectIndexedNotZeroPage(token));
                }
                return OperationResult.Success();
            }),
            (NodePattern.Single(TokenType.RightParen), null),
            (NodePattern.Single(TokenType.Comma), null),
            (NodePattern.Single(TokenType.RegisterY), null)
        };

        protected override AbstractStatement Construct(List<Token> tokens) =>
            new InstructionStatementIndirectIndexed(tokens);
    }
}