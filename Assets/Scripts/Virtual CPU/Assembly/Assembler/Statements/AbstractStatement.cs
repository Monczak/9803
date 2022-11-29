using System;
using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements
{
    public abstract class AbstractStatement
    {
        private readonly List<Token> tokens;
        private int current;

        protected internal delegate void TokenHandler(Token token);
        
        protected internal abstract List<(TokenType type, TokenHandler handler)> Pattern { get; }

        protected AbstractStatement()
        {
        }

        protected AbstractStatement(List<Token> tokens)
        {
            this.tokens = tokens;
            current = 0;
        }

        protected abstract AbstractStatement Construct(List<Token> tokens);

        public AbstractStatement Build(List<Token> theTokens)
        {
            return Construct(theTokens).ConsumePattern();
        }

        private Token Consume()
        {
            return tokens[current++];
        }

        private bool IsAtEnd() => current >= tokens.Count;

        private AbstractStatement ConsumePattern()
        {
            while (!IsAtEnd())
            {
                Token token = Consume();
                if ((token.Type & Pattern[current - 1].type) == 0)
                    return null;
                if (Pattern[current - 1].handler is not null)
                    Pattern[current - 1].handler(token);
            }
            return this;
        }
    }

    public abstract class IntermediateStatement : AbstractStatement
    {
        public abstract Type FollowedBy { get; }
        
        protected IntermediateStatement(List<Token> tokens) : base(tokens)
        {
        }
    }
    
    public abstract class FinalStatement : AbstractStatement
    {
        protected FinalStatement(List<Token> tokens) : base(tokens)
        {
        }
    }
}