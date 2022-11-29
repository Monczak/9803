﻿using System;
using System.Collections.Generic;
using System.Linq;
using NineEightOhThree.Utilities;
using NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public static class Parser
    {
        private static int current;
        private static int line;

        private static List<AbstractStatement> statements;

        private static List<Token> source;

        private static GrammarGraph graph;

        public static bool HadError { get; private set; }

        public delegate void ErrorHandler(ParserError? ex);
        private static event ErrorHandler OnError;


        private class GrammarGraph
        {
            public GrammarNode Root { get; }

            public GrammarGraph()
            {
                Root = new GrammarNode(null);
            }

            public class GrammarNode
            {
                public TokenType? TokenType { get; }
                public bool IsFinal { get; }
                public AbstractStatement Statement { get; }
                public Type StatementType { get; set; }
                public List<GrammarNode> Children { get; }

                public GrammarNode(TokenType? type)
                {
                    TokenType = type;
                    Children = new List<GrammarNode>();
                    IsFinal = false; 
                }

                public GrammarNode(TokenType? type, AbstractStatement stmt)
                {
                    TokenType = type;
                    Children = new List<GrammarNode>();
                    Statement = stmt;
                    IsFinal = true;
                }

                private bool Equals(GrammarNode other)
                {
                    return TokenType == other.TokenType;
                }

                public override bool Equals(object obj)
                {
                    if (ReferenceEquals(null, obj)) return false;
                    if (ReferenceEquals(this, obj)) return true;
                    return obj.GetType() == GetType() && Equals((GrammarNode)obj);
                }

                public override int GetHashCode()
                {
                    return HashCode.Combine((int?)TokenType);
                }
            }

            private static List<T> FindAllStatementTypes<T>() where T : AbstractStatement =>
                System.Reflection.Assembly.GetAssembly(typeof(T)).GetTypes()
                    .Where(type => type.IsClass && !type.IsAbstract && type.IsSealed && type.IsSubclassOf(typeof(T)))
                    .Select(t => (T)Activator.CreateInstance(t, new object[] {null}))
                    .ToList();

            private void AddPath(AbstractStatement stmt, Type connectTo = null)
            {
                Queue<TokenType> typeQueue = new(stmt.Pattern.Select(p => p.type));
                GrammarNode currentNode = Root;
                while (typeQueue.Count > 0)
                {
                    TokenType type = typeQueue.Dequeue();

                    GrammarNode node = typeQueue.Count == 0 ? new GrammarNode(type, stmt) : new GrammarNode(type);

                    if (typeQueue.Count == 0 && connectTo is not null)
                    {
                        currentNode.Children.Add(node);
                        node.Children.Add(FindEarliestOfType(Root, (node.Statement as IntermediateStatement)?.FollowedBy));
                    }
                    else
                    {
                        GrammarNode child = currentNode.Children.Find(n => n.Equals(node));
                        if (child == null)
                        {
                            currentNode.Children.Add(node);
                            currentNode = node;
                        }
                        else
                        {
                            currentNode = child;
                        }
                    }
                }
            }

            private void UpdateBaseTypes(GrammarNode node)
            {
                if (node.Children.Count == 0)
                    node.StatementType = node.Statement.GetType();
                else
                {
                    foreach (GrammarNode child in node.Children)
                    {
                        UpdateBaseTypes(child);
                    }

                    if (!Equals(node, Root))
                        node.StatementType =
                            TypeUtils.MostDerivedCommonBase(node.Children.Select(child => child.StatementType));
                }
            }

            private static GrammarNode FindEarliestOfType(GrammarNode node, Type type)
            {
                if (node.Children.Count == 0) return null;
                if (node.StatementType == type) return node;
                foreach (GrammarNode child in node.Children)
                {
                    GrammarNode potentialMatch = FindEarliestOfType(child, type);
                    if (potentialMatch is not null) return potentialMatch;
                }

                return null;
            }
            
            public static GrammarGraph Build()
            {
                GrammarGraph graph = new GrammarGraph();

                List<AbstractStatement> statements = FindAllStatementTypes<AbstractStatement>();
                foreach (var stmt in statements)
                {
                    switch (stmt)
                    {
                        case FinalStatement s:
                            graph.AddPath(s);
                            break;
                        case IntermediateStatement s:
                            graph.UpdateBaseTypes(graph.Root);
                            graph.AddPath(s, s.FollowedBy);
                            break;
                    }
                }
                
                return graph;
            }
        }
        
        public static List<AbstractStatement> Parse(List<Token> tokens)
        {
            statements = new List<AbstractStatement>();
            source = tokens;
            line = 1;
            current = 0;
            HadError = false;
            
            graph ??= GrammarGraph.Build();

            CreateStatements();
            CreateLabels();

            return statements;
            // return HadError ? null : statements;
        }
        
        private static void CreateStatements()
        {
            while (!IsAtEnd())
            {
                try
                {
                    ParsingResult<AbstractStatement> stmt = ScanStatement();
                    if (stmt.Failed)
                    {
                        OnError?.Invoke(stmt.TheError);
                        HadError = true;
                        Synchronize();
                    }
                }
                catch (Exception e)
                {
                    throw new InternalErrorException("Parser error", e);
                }
            }
        }

        // TODO: Create labels from DeclareLabel statements and infer their usage based on the next statement (allow label at end of file?)
        private static void CreateLabels()
        {
            
        }

        private static void Synchronize()
        {
            while (Peek(-1).Type is not TokenType.Newline or TokenType.EndOfFile) Advance();
        }

        private static ParsingResult<AbstractStatement> ScanStatement()
        {
            List<Token> lineTokens = new();

            GrammarGraph.GrammarNode currentNode = graph.Root;

            while (!IsAtEnd())
            {
                Token token = Advance();

                if (token.Type is TokenType.Newline or TokenType.EndOfFile && !currentNode.IsFinal)
                {
                    return ParsingResult<AbstractStatement>.Error(ErrorExpectedTokens(currentNode, token));
                }
                
                if (token.Type is TokenType.Newline or TokenType.EndOfFile)
                {
                    ParsingResult<AbstractStatement> stmt = currentNode.Statement.Build(lineTokens);
                    line++;
                    
                    if (stmt.Failed) return stmt;

                    AddStatement(stmt.Result);
                    lineTokens.Clear();
                    currentNode = graph.Root;
                }
                else
                {
                    lineTokens.Add(token);
                    
                    bool found = false;
                    foreach (GrammarGraph.GrammarNode child in currentNode.Children)
                    {
                        if ((child.TokenType & token.Type) != 0)
                        {
                            found = true;
                            currentNode = child;
                            break;
                        }
                    }

                    if (!found)
                    {
                        return ParsingResult<AbstractStatement>.Error(ErrorExpectedTokens(currentNode, token));
                    }

                    if (currentNode.Statement is IntermediateStatement stmt)
                    {
                        ParsingResult<AbstractStatement> s = stmt.Build(lineTokens);
                        if (s.Failed) return s;
                        
                        AddStatement(s.Result);
                        lineTokens.Clear();
                    }
                }
            }
            return ParsingResult<AbstractStatement>.Success();
        }

        private static ParserError ErrorExpectedTokens(GrammarGraph.GrammarNode currentNode, Token token)
        {
            TokenType? expectedType = currentNode.Children
                .Select(n => n.TokenType)
                .Aggregate((acc, t) => acc | t);
            if (!expectedType.HasValue)
                throw new InternalErrorException("Expected type was null");

            return new ParserError( SyntaxErrors.ExpectedGot(
                expectedType.Value,
                token.Type
            ), token);
        }

        public static void RegisterErrorHandler(ErrorHandler handler)
        {
            if (OnError == null)
                OnError += handler;
        }

        private static Token Advance()
        {
            return source[current++];
        }

        private static Token Peek(int offset = 0) => IsAtEnd() ? new Token {Type = TokenType.EndOfFile} : source[current + offset];

        private static void AddStatement(AbstractStatement statement)
        {
            statements.Add(statement);
        }

        private static bool IsAtEnd() => current >= source.Count;
    }
}