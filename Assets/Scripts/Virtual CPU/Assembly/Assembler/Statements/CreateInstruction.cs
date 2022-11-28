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

        private IEnumerable<(CPUInstruction instruction, AddressingMode addressingMode, CPUInstructionMetadata metadata)> InstructionCandidates
        {
            get;
            set;
        }

        protected void FindCandidates(Token token, Predicate<AddressingMode> modePredicate)
        {
            try
            {
                InstructionCandidates = CPUInstructionRegistry
                    .GetInstructions(token.Content)
                    .Where(info => modePredicate(info.addressingMode));
            }
            catch (UnknownInstructionException)
            {
                throw new SyntaxErrorException(SyntaxErrors.UnknownInstruction(token), token);
            }
            
            if (!InstructionCandidates.Any())
                throw new SyntaxErrorException(
                    SyntaxErrors.AddressingModeNotSupported(token, AddressingMode), token);
        }

        protected void FindInstruction(Token token)
        {
            FindCandidates(token, mode => mode == AddressingMode);
            MatchInstructionFromFound(AddressingMode);
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

        protected void SetOperand(Token token)
        {
            Operand = token.Type switch
            {
                TokenType.Number => new Operand((ushort)token.Literal),
                TokenType.Identifier => new Operand(token.Content),
                _ => Operand
            };
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
            (TokenType.Identifier, token => FindCandidates(token, mode => mode is AddressingMode.Absolute or AddressingMode.ZeroPage)),
            (TokenType.Number | TokenType.Identifier, token =>
            {
                SetOperand(token);
                if (Operand.IsDefined)
                    MatchInstructionFromFound(Operand.Number < 256 ? AddressingMode.ZeroPage : AddressingMode.Absolute);
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
            (TokenType.Identifier, token => FindCandidates(token, mode => mode is AddressingMode.AbsoluteX or AddressingMode.AbsoluteY or AddressingMode.ZeroPageX or AddressingMode.ZeroPageY)),
            (TokenType.Number | TokenType.Identifier, SetOperand),
            (TokenType.Comma, null),
            (TokenType.RegisterX | TokenType.RegisterY, token =>
            {
                if (!Operand.IsDefined) return;
                switch (Operand.Number < 256, token.Type)
                {
                    case (false, TokenType.RegisterX): MatchInstructionFromFound(AddressingMode.AbsoluteX); break;
                    case (false, TokenType.RegisterY): MatchInstructionFromFound(AddressingMode.AbsoluteY); break;
                    case (true, TokenType.RegisterX): MatchInstructionFromFound(AddressingMode.ZeroPageX); break;
                    case (true, TokenType.RegisterY): MatchInstructionFromFound(AddressingMode.ZeroPageY); break;
                    default: throw new SyntaxErrorException(SyntaxErrors.RegisterNotXY(token), token);
                }
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
                if (Operand.IsDefined)
                {
                    if (Operand.Number > 255)
                        throw new SyntaxErrorException(
                            SyntaxErrors.IndexedIndirectNotZeroPage(token), token);
                }
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
                if (Operand.IsDefined)
                {
                    if (Operand.Number > 255)
                        throw new SyntaxErrorException(
                            SyntaxErrors.IndirectIndexedNotZeroPage(token), token);
                }
            }),
            (TokenType.RightParen, null),
            (TokenType.Comma, null),
            (TokenType.RegisterY, null)
        };

        protected override AbstractStatement Construct(List<Token> tokens) =>
            new CreateInstructionIndirectIndexed(tokens);
    }
}