using System;
using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements
{
    public abstract class AbstractStatement
    {
        private readonly List<Token> tokens;
        private int current;
        private int currentPattern;

        protected internal delegate OperationResult TokenHandler(Token token);
        
        protected internal abstract List<(NodePattern pattern, TokenHandler handler)> Pattern { get; }

        protected AbstractStatement()
        {
        }

        protected AbstractStatement(List<Token> tokens)
        {
            this.tokens = tokens;
            current = 0;
        }

        protected abstract AbstractStatement Construct(List<Token> tokens);

        public OperationResult<AbstractStatement> Build(List<Token> theTokens)
        {
            return Construct(theTokens).ConsumePattern();
        }

        private Token Consume()
        {
            return tokens[current++];
        }

        private bool IsAtEnd() => current >= tokens.Count;

        private OperationResult<AbstractStatement> ConsumePattern()
        {
            current = 0;
            currentPattern = 0;

            while (!IsAtEnd())
            {
                Token token = Consume();
                if (currentPattern + 1 < Pattern.Count && !Pattern[currentPattern + 1].pattern.Cycle && Pattern[currentPattern].pattern.Cycle)
                    currentPattern++;

                (NodePattern pattern, TokenHandler handler) = Pattern[currentPattern];
                if ((token.Type & pattern.TokenType) == 0)
                {
                    TokenType? patternTokenType = pattern.TokenType;
                    if (patternTokenType != null)
                        return OperationResult<AbstractStatement>.Error(SyntaxErrors.ExpectedGot(token,
                            patternTokenType.Value, token.Type));
                    throw new InternalErrorException("Pattern token type was null");
                }

                if (handler is not null)
                {
                    OperationResult result = handler(token);
                    if (result.Failed)
                        return OperationResult<AbstractStatement>.Error(result.TheError);
                }

                if (!pattern.Cycle) currentPattern++;
            }
            return OperationResult<AbstractStatement>.Success(this);
        }
    }

    public abstract class IntermediateStatement : AbstractStatement
    {
        public abstract List<Type> FollowedBy { get; }
        
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