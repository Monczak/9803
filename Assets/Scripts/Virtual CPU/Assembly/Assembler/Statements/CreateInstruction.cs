using System;
using System.Collections.Generic;
using System.Linq;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements
{
    public abstract class CreateInstruction : FinalStatement
    {
        public CPUInstruction Instruction { get; protected set; }
        public AddressingMode AddressingMode { get; protected set; }
        public CPUInstructionMetadata Metadata => Instruction.Metadata[AddressingMode];
        
        protected CreateInstruction(List<Token> tokens) : base(tokens)
        {
            
        }

        protected IEnumerable<(CPUInstruction instruction, AddressingMode addressingMode, CPUInstructionMetadata metadata)> InstructionCandidates
        {
            get;
            set;
        }
        
        protected ParsingResult FindCandidates(Token token, AddressingMode modeFlags)
        {
            try
            {
                InstructionCandidates = CPUInstructionRegistry
                    .GetInstructions(token.Content)
                    .Where(info => (info.addressingMode & modeFlags) != 0);
            }
            catch (UnknownInstructionException)
            {
                return ParsingResult.Error(new ParserError(SyntaxErrors.UnknownInstruction(token), token));
            }
            
            if (!InstructionCandidates.Any())
                return ParsingResult.Error(new ParserError(
                    SyntaxErrors.AddressingModeNotSupported(token, AddressingMode), token));
            
            return ParsingResult.Success();
        }

        protected ParsingResult FindInstruction(Token token)
        {
            ParsingResult candidateResult = FindCandidates(token, AddressingMode);
            if (candidateResult.Failed)
                return candidateResult;
            
            MatchInstructionFromFound(AddressingMode);
            return ParsingResult.Success();
        }

        protected void MatchInstructionFromFound(AddressingMode addressingMode)
        {
            Instruction = InstructionCandidates.First(info => info.addressingMode == addressingMode).instruction;
            AddressingMode = addressingMode;
        }
    }

    public abstract class CreateInstructionOperand : CreateInstruction
    {
        public Operand Operand { get; protected set; }
        
        protected CreateInstructionOperand(List<Token> tokens) : base(tokens)
        {
        }

        protected ParsingResult SetOperand(Token token)
        {
            Operand = token.Type switch
            {
                TokenType.Number => new Operand((ushort)token.Literal),
                TokenType.Identifier => new Operand(token.Content),
                _ => Operand
            };
            return ParsingResult.Success();
        }
    }

    public sealed class CreateInstructionImplied : CreateInstruction
    {
        public CreateInstructionImplied(List<Token> tokens) : base(tokens)
        {
            AddressingMode = AddressingMode.Implied;
        }

        protected internal override List<(TokenType type, TokenHandler handler)> Pattern => new()
        {
            (TokenType.Identifier, FindInstruction),
        };

        protected override AbstractStatement Construct(List<Token> tokens) =>
            new CreateInstructionImplied(tokens);
    }

    public sealed class CreateInstructionAccumulator : CreateInstruction
    {
        public CreateInstructionAccumulator(List<Token> tokens) : base(tokens)
        {
            AddressingMode = AddressingMode.Accumulator;
        }

        protected internal override List<(TokenType type, TokenHandler handler)> Pattern => new()
        {
            (TokenType.Identifier, FindInstruction),
            (TokenType.RegisterA, null)
        };

        protected override AbstractStatement Construct(List<Token> tokens) =>
            new CreateInstructionAccumulator(tokens);
    }

    public sealed class CreateInstructionAbsolute : CreateInstructionOperand
    {
        public CreateInstructionAbsolute(List<Token> tokens) : base(tokens)
        {
            
        }

        protected internal override List<(TokenType type, TokenHandler handler)> Pattern => new()
        {
            (TokenType.Identifier, token => FindCandidates(token, AddressingMode.Absolute | AddressingMode.ZeroPage | AddressingMode.Relative)),
            (TokenType.Number | TokenType.Identifier, token =>
            {
                SetOperand(token);

                // Assume that if the first instruction in the candidates is a branch, all other are as well
                // (this enumerable contains multiple types of one instruction)
                bool isBranch = InstructionCandidates.First().instruction.IsBranch;

                if (isBranch)
                {
                    MatchInstructionFromFound(AddressingMode.Relative);
                }
                else if (Operand.IsDefined && Operand.Number < 256)
                {
                    MatchInstructionFromFound(AddressingMode.ZeroPage);
                }
                else
                {
                    MatchInstructionFromFound(AddressingMode.Absolute);
                }
                
                return ParsingResult.Success();
            })
        };
        
        protected override AbstractStatement Construct(List<Token> tokens) => new CreateInstructionAbsolute(tokens);
    }

    public sealed class CreateInstructionImmediateOp : CreateInstructionOperand
    {
        public CreateInstructionImmediateOp(List<Token> tokens) : base(tokens)
        {
            AddressingMode = AddressingMode.Immediate;
        }

        protected internal override List<(TokenType type, TokenHandler handler)> Pattern => new()
        {
            (TokenType.Identifier, FindInstruction),
            (TokenType.ImmediateOp, null),
            (TokenType.Number | TokenType.Identifier, SetOperand)
        };

        protected override AbstractStatement Construct(List<Token> tokens) => new CreateInstructionImmediateOp(tokens);
    }

    public sealed class CreateInstructionAbsoluteIndexed : CreateInstructionOperand
    {
        public CreateInstructionAbsoluteIndexed(List<Token> tokens) : base(tokens)
        {
            
        }

        protected internal override List<(TokenType type, TokenHandler handler)> Pattern => new()
        {
            (TokenType.Identifier, token => FindCandidates(token, AddressingMode.AbsoluteX | AddressingMode.AbsoluteY | AddressingMode.ZeroPageX | AddressingMode.ZeroPageY)),
            (TokenType.Number | TokenType.Identifier, SetOperand),
            (TokenType.Comma, null),
            (TokenType.RegisterX | TokenType.RegisterY, token =>
            {
                switch (Operand.IsDefined, Operand.IsDefined && Operand.Number < 256, token.Type)
                {
                    case (_, false, TokenType.RegisterX): MatchInstructionFromFound(AddressingMode.AbsoluteX); break;
                    case (_, false, TokenType.RegisterY): MatchInstructionFromFound(AddressingMode.AbsoluteY); break;
                    case (true, true, TokenType.RegisterX): MatchInstructionFromFound(AddressingMode.ZeroPageX); break;
                    case (true, true, TokenType.RegisterY): MatchInstructionFromFound(AddressingMode.ZeroPageY); break;
                    default: return ParsingResult.Error(new ParserError(SyntaxErrors.RegisterNotXY(token), token));
                }
                return ParsingResult.Success();
            })
        };

        protected override AbstractStatement Construct(List<Token> tokens) =>
            new CreateInstructionAbsoluteIndexed(tokens);
    }

    public sealed class CreateInstructionIndirect : CreateInstructionOperand
    {
        public CreateInstructionIndirect(List<Token> tokens) : base(tokens)
        {
            AddressingMode = AddressingMode.Indirect;
        }

        protected internal override List<(TokenType type, TokenHandler handler)> Pattern => new()
        {
            (TokenType.Identifier, FindInstruction),
            (TokenType.LeftParen, null),
            (TokenType.Number | TokenType.Identifier, SetOperand),
            (TokenType.RightParen, null)
        };

        protected override AbstractStatement Construct(List<Token> tokens) =>
            new CreateInstructionIndirect(tokens);
    }

    public sealed class CreateInstructionIndexedIndirect : CreateInstructionOperand
    {
        public CreateInstructionIndexedIndirect(List<Token> tokens) : base(tokens)
        {
            AddressingMode = AddressingMode.IndexedIndirect;
        }

        protected internal override List<(TokenType type, TokenHandler handler)> Pattern => new()
        {
            (TokenType.Identifier, FindInstruction),
            (TokenType.LeftParen, null),
            (TokenType.Number | TokenType.Identifier, token =>
            {
                SetOperand(token);
                if (Operand.IsDefined)  // If the operand is undefined (label), check if valid later
                {
                    if (Operand.Number > 255)
                        return ParsingResult.Error(new ParserError(
                            SyntaxErrors.IndexedIndirectNotZeroPage(token), token));
                }
                return ParsingResult.Success();
            }),
            (TokenType.Comma, null),
            (TokenType.RegisterX, null),
            (TokenType.RightParen, null),
        };

        protected override AbstractStatement Construct(List<Token> tokens) =>
            new CreateInstructionIndexedIndirect(tokens);
    }

    public sealed class CreateInstructionIndirectIndexed : CreateInstructionOperand
    {
        public CreateInstructionIndirectIndexed(List<Token> tokens) : base(tokens)
        {
            AddressingMode = AddressingMode.IndirectIndexed;
        }

        protected internal override List<(TokenType type, TokenHandler handler)> Pattern => new()
        {
            (TokenType.Identifier, FindInstruction),
            (TokenType.LeftParen, null),
            (TokenType.Number | TokenType.Identifier, token =>
            {
                SetOperand(token);
                if (Operand.IsDefined)  // If the operand is undefined (label), check if valid later
                {
                    if (Operand.Number > 255)
                        return ParsingResult.Error(new ParserError(
                            SyntaxErrors.IndirectIndexedNotZeroPage(token), token));
                }
                return ParsingResult.Success();
            }),
            (TokenType.RightParen, null),
            (TokenType.Comma, null),
            (TokenType.RegisterY, null)
        };

        protected override AbstractStatement Construct(List<Token> tokens) =>
            new CreateInstructionIndirectIndexed(tokens);
    }
}