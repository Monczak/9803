using System.Collections.Generic;
using NineEightOhThree.VirtualCPU.Assembly.Assembler.Directives;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements
{
    public abstract class DirectiveStatement : FinalStatement
    {
        public Directive Directive { get; protected set; }

        protected DirectiveStatement(List<Token> tokens) : base(tokens)
        {
        }
        
        protected OperationResult MatchDirective(Token token)
        {
            if (!DirectiveRegistry.TryGetDirective(token.Content[1..], out Directive directive))
                return OperationResult.Error(SyntaxErrors.UnknownDirective(token));

            Directive = directive;
            return OperationResult.Success();
        }
    }

    public abstract class DirectiveStatementOperands : DirectiveStatement
    {
        public List<Operand> Args { get; }
        
        protected DirectiveStatementOperands(List<Token> tokens) : base(tokens)
        {
            Args = new List<Operand>();
        }

        protected OperationResult AddOperand(Token token)
        {
            switch (token.Type)
            {
                case TokenType.Number:
                    ushort n = (ushort)token.Literal;
                    Args.Add(new Operand(token, n));
                    break;
                case TokenType.Identifier:
                    Args.Add(new Operand(token, token.Content));
                    break;
            }
                
            return OperationResult.Success();
        }
        
        
        public override void FinalizeStatement()
        {
            Directive = Directive.Construct(Args);
        }
    }
    
    public sealed class NullaryDirectiveStatement : DirectiveStatement
    {
        public NullaryDirectiveStatement(List<Token> tokens) : base(tokens)
        {
        }

        protected internal override List<(NodePattern pattern, TokenHandler handler)> Pattern => new()
        {
            (NodePattern.Single(TokenType.Directive), MatchDirective)
        };

        protected override AbstractStatement Construct(List<Token> tokens) => new NullaryDirectiveStatement(tokens);
    }
    
    public sealed class VariadicDirectiveStatement : DirectiveStatementOperands
    {
        public VariadicDirectiveStatement(List<Token> tokens) : base(tokens)
        {
        }

        protected internal override List<(NodePattern pattern, TokenHandler handler)> Pattern => new()
        {
            (NodePattern.Single(TokenType.Directive), MatchDirective),
            (NodePattern.Multiple(TokenType.Number | TokenType.Identifier), AddOperand)
        };

        protected override AbstractStatement Construct(List<Token> tokens) => new VariadicDirectiveStatement(tokens);
    }
}