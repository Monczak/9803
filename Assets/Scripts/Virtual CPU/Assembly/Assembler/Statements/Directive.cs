using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements
{
    public abstract class Directive : FinalStatement
    {
        public string Name { get; private set; }

        protected Directive(List<Token> tokens) : base(tokens)
        {
            
        }

        protected OperationResult MatchDirective(Token token, string name)
        {
            string tokenName = token.Content[1..];
            if (tokenName != name) return OperationResult.Error(SyntaxErrors.UnknownDirective(token));
            Name = tokenName;
            return OperationResult.Success();
        }
    }

    public abstract class Directive1Arg<T> : Directive
    {
        public T Arg { get; private set; }
        
        protected Directive1Arg(List<Token> tokens) : base(tokens)
        {
            
        }
    }

    public abstract class VariadicDirective<T> : Directive
    {
        public List<T> Args { get; private set; }

        protected VariadicDirective(List<Token> tokens) : base(tokens)
        {
            Args = new List<T>();
        }
    }

    public sealed class ByteDirective : VariadicDirective<byte>
    {
        public ByteDirective(List<Token> tokens) : base(tokens)
        {
            
        }

        protected internal override List<(NodePattern pattern, TokenHandler handler)> Pattern => new()
        {
            (NodePattern.Single(TokenType.Directive), token => MatchDirective(token, "byte")),
            (NodePattern.Multiple(TokenType.Number), token =>
            {
                ushort n = (ushort)token.Literal;
                if (n >= 256)
                    return OperationResult.Error(SyntaxErrors.OperandNotByte(token));
                return OperationResult.Success();
            })
        };
        protected override AbstractStatement Construct(List<Token> tokens) => new ByteDirective(tokens);
    }
}