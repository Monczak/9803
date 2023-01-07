using System;
using System.Collections.Generic;
using System.Linq;
using NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public partial class Parser : LogErrorProducer
    {
        private int current;

        private List<AbstractStatement> statements;

        private List<Token> source;

        private GrammarGraph graph;

        public bool HadError { get; private set; }


        public List<AbstractStatement> Parse(List<Token> tokens)
        {
            statements = new List<AbstractStatement>();
            source = tokens;
            current = 0;
            HadError = false;

            AssemblerCache.GrammarGraph ??= GrammarGraph.Build();
            graph = AssemblerCache.GrammarGraph;

            CreateStatements();

            return statements;
            // return HadError ? null : statements;
        }
        
        private void CreateStatements()
        {
            while (!IsAtEnd())
            {
                try
                {
                    OperationResult<AbstractStatement> stmt = ScanStatement();
                    if (stmt.Failed)
                    {
                        MakeError(stmt.TheError);
                        HadError = true;
                        Synchronize();
                    }
                }
                catch (Exception e)
                {
                    throw new InternalErrorException($"Parser error: {e}", e);
                }
            }
        }

        private void Synchronize()
        {
            while (!IsAtEnd() && Peek(-1).Type is not TokenType.Newline or TokenType.EndOfFile) Advance();
        }

        private OperationResult<AbstractStatement> ScanStatement()
        {
            List<Token> lineTokens = new();
            bool ignoreNewlines = false;

            GrammarGraph.GrammarNode currentNode = graph.Root;

            while (!IsAtEnd())
            {
                Token token = Advance();

                if (!IsAtEnd() && ignoreNewlines && token.Type is TokenType.Newline or TokenType.EndOfFile)
                    continue;
                ignoreNewlines = false;
                
                if (token.Type is not (TokenType.Newline or TokenType.EndOfFile)) lineTokens.Add(token);

                if (token.Type is TokenType.Newline or TokenType.EndOfFile && !currentNode.IsFinal)
                {
                    if (lineTokens.Count == 0)  // If parsing an empty line
                        return OperationResult<AbstractStatement>.Success();
                    
                    return OperationResult<AbstractStatement>.Error(ErrorExpectedTokens(currentNode, token));
                }
                
                if (token.Type is TokenType.Newline or TokenType.EndOfFile)
                {
                    // If the last statement parsed is an IntermediateStatement, don't add it again
                    if (currentNode.Statement is IntermediateStatement && IsAtEnd()) continue;

                    OperationResult<AbstractStatement> stmt = currentNode.Statement.Build(lineTokens);

                    if (stmt.Failed) return stmt;

                    AddStatement(stmt.Result);
                    lineTokens.Clear();
                    currentNode = graph.Root;
                }
                else
                {
                    if (currentNode.Children.Count == 0)
                    {
                        return OperationResult<AbstractStatement>.Error(SyntaxErrors.UnexpectedToken(token));
                    }
                    
                    bool found = false;
                    foreach (GrammarGraph.GrammarNode child in currentNode.Children)
                    {
                        if (!child.Pattern.TokenType.HasValue)
                            throw new InternalErrorException("Pattern token type was null");
                        if ((child.Pattern.TokenType.Value & token.Type) != 0)
                        {
                            found = true;
                            currentNode = child;
                            break;
                        }
                    }

                    if (!found)
                    {
                        return OperationResult<AbstractStatement>.Error(ErrorExpectedTokens(currentNode, token));
                    }

                    if (currentNode.Statement is IntermediateStatement stmt)
                    {
                        OperationResult<AbstractStatement> s = stmt.Build(lineTokens);
                        if (s.Failed) return s;
                        
                        stmt.FinalizeStatement();
                        
                        AddStatement(s.Result);
                        lineTokens.Clear();

                        ignoreNewlines = true;
                    }
                }
            }
            return OperationResult<AbstractStatement>.Success();
        }

        private static AssemblerError ErrorExpectedTokens(GrammarGraph.GrammarNode currentNode, Token token)
        {
            TokenType? expectedType = currentNode.Children
                .Select(n => n.Pattern.TokenType)
                .Aggregate((acc, t) => acc | t);
            if (!expectedType.HasValue)
                throw new InternalErrorException("Expected type was null");

            return SyntaxErrors.ExpectedGot(token, 
                expectedType.Value,
                token.Type
            );
        }

        private Token Advance()
        {
            return source[current++];
        }

        private Token Peek(int offset = 0) => IsAtEnd() ? new Token {Type = TokenType.EndOfFile} : source[current + offset];

        private void AddStatement(AbstractStatement statement)
        {
            statements.Add(statement);
        }

        private bool IsAtEnd() => current >= source.Count;
    }
}