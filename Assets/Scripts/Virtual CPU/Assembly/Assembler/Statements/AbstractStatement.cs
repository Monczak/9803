using System;
using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements
{
    public abstract class AbstractStatement
    {
        private readonly List<Token> tokens;
        private int current;

        protected internal delegate ParsingResult TokenHandler(Token token);
        
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

        public ParsingResult<AbstractStatement> Build(List<Token> theTokens)
        {
            return Construct(theTokens).ConsumePattern();
        }

        private Token Consume()
        {
            return tokens[current++];
        }

        private bool IsAtEnd() => current >= tokens.Count;

        private ParsingResult<AbstractStatement> ConsumePattern()
        {
            while (!IsAtEnd())
            {
                Token token = Consume();
                if ((token.Type & Pattern[current - 1].type) == 0)
                    return ParsingResult<AbstractStatement>.Error(new ParserError(SyntaxErrors.ExpectedGot(Pattern[current - 1].type, token.Type), token));
                if (Pattern[current - 1].handler is not null)
                {
                    ParsingResult result = Pattern[current - 1].handler(token);
                    if (result.Failed)
                        return ParsingResult<AbstractStatement>.Error(result.TheError);
                }
            }
            return ParsingResult<AbstractStatement>.Success(this);
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