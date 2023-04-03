using System;
using System.Collections.Generic;
using System.Linq;
using NineEightOhThree.Utilities;
using NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements;
using NineEightOhThree.VirtualCPU.Assembly.Assembler.Directives;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public partial class Parser : LogErrorProducer
    {
        private int current;

        private List<AbstractStatement> statements;

        private List<Token> source;
        private string fileName;

        private string currentNamespace;

        private GrammarGraph graph;

        public bool HadError { get; private set; }

        private HashSet<string> useChain;


        public List<AbstractStatement> Parse(List<Token> tokens, string currentNamespace = "", HashSet<string> useChain = null)
        {
            statements = new List<AbstractStatement>();
            source = tokens;
            current = 0;
            HadError = false;
            fileName = tokens[0].ResourceLocation;

            this.currentNamespace = currentNamespace;

            if (useChain is null)
            {
                this.useChain = new HashSet<string> { fileName };
            }
            else
            {
                this.useChain = new HashSet<string>(useChain) { fileName };
            }

            AssemblerCache.GrammarGraph = GrammarGraph.Build();
            graph = AssemblerCache.GrammarGraph;

            CreateStatements();
            
            HandleIncludes();

            return statements;
            // return HadError ? null : statements;
        }

        private void HandleIncludes()
        {
            // Replace includes with AbstractStatements from the corresponding file
            for (int i = 0; i < statements.Count; i++)
            {
                AbstractStatement stmt = statements[i];
                
                var result = HandleInclude(stmt, i);
                if (result.Failed)
                {
                    MakeError((AssemblerError)result.TheError);
                    HadError = true;
                    Synchronize();
                }
            }
        }

        private OperationResult HandleInclude(AbstractStatement stmt, int index)
        {
            if (stmt is DirectiveStatementOperands { Directive: IncludeDirective directive } s)
            {
                string @namespace = s.Args[0].Token.Content; 
                directive.Setup(@namespace);
                
                string location =
                    PathUtils.GetAbsoluteResourceLocation<TextAsset>(directive.IncludedResourceLocation, fileName);
                MakeLog($"Include {location}");
                
                useChain ??= new HashSet<string>();
                if (useChain.Contains(location))
                    return OperationResult.Error(SyntaxErrors.CircularDependency(s.Tokens[1], location));

                // Try to open resource at the specified location and parse the code
                TextAsset asset = Resources.Load<TextAsset>(location);
                if (asset is null)
                    return OperationResult.Error(SyntaxErrors.InvalidIncludedResourceLocation(s.Tokens[1], location));
                string code = asset.text;
                
                // TODO: Check if any changes to the code were made, if not, use cached List<AbstractStatement>
                // And if so, reparse the code and save to cache

                if (!AssemblerCache.TryGetParseCache(location, code, out var stmts))
                {
                    Lexer subLexer = new Lexer();
                    Parser subParser = new Parser();
                    subLexer.RegisterLogHandlers(LogHandlers);
                    subLexer.RegisterErrorHandlers(ErrorHandlers);
                    subParser.RegisterLogHandlers(LogHandlers);
                    subParser.RegisterErrorHandlers(ErrorHandlers);

                    var tokens = subLexer.Lex(code, location);
                    if (subLexer.HadError) return OperationResult.Success();

                    stmts = subParser.Parse(tokens, $"{currentNamespace}{(currentNamespace == string.Empty ? "" : ".")}{@namespace}", useChain);
                    if (subParser.HadError) return OperationResult.Success();
                    
                    AssemblerCache.CacheParseResults(location, code, stmts);
                }
                else
                {
                    MakeLog($"Using cached {location}");
                }
                
                statements.ReplaceRange(index, stmts);
            }
            return OperationResult.Success();
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
                        MakeError((AssemblerError)stmt.TheError);
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

                    OperationResult<AbstractStatement> stmt = currentNode.Statement.Build(lineTokens, currentNamespace);

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
                        OperationResult<AbstractStatement> s = stmt.Build(lineTokens, currentNamespace);
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