using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements
{
    public abstract class DirectiveStatement : FinalStatement
    {
        public string Name { get; private set; }

        protected DirectiveStatement(List<Token> tokens) : base(tokens)
        {
            
        }

        protected OperationResult MatchDirective(Token token)
        {
            Name = token.Content[1..];
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
                    Args.Add(new Operand(n));
                    break;
                case TokenType.Identifier:
                    Args.Add(new Operand(token.Content));
                    break;
            }
                
            return OperationResult.Success();
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