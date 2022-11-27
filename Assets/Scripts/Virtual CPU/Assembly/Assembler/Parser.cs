using System;
using System.Collections.Generic;
using System.Linq;
using NineEightOhThree.Utilities;
using NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public static class Parser
    {
        private static int current;
        private static int line;

        private static List<AbstractStatement> statements;

        private static List<Token> source;

        private class GrammarGraph
        {
            private GrammarNode root;

            public GrammarGraph()
            {
                root = new GrammarNode(null);
            }

            private class GrammarNode
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
                GrammarNode currentNode = root;
                while (typeQueue.Count > 0)
                {
                    TokenType type = typeQueue.Dequeue();

                    GrammarNode node = typeQueue.Count == 0 ? new GrammarNode(type, stmt) : new GrammarNode(type);

                    if (typeQueue.Count == 0 && connectTo is not null)
                    {
                        currentNode.Children.Add(node);
                        node.Children.Add(FindEarliestOfType(root, (node.Statement as IntermediateStatement)?.FollowedBy));
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

                    if (!Equals(node, root))
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
                    Debug.Log($"{stmt.GetType().Name} - {string.Join(' ', stmt.Pattern.Select(p => p.type.ToString()))}");
                    switch (stmt)
                    {
                        case FinalStatement s:
                            graph.AddPath(s);
                            break;
                        case IntermediateStatement s:
                            graph.UpdateBaseTypes(graph.root);
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

            GrammarGraph graph = GrammarGraph.Build(); 

            while (!IsAtEnd())
            {
                try
                {
                    ScanStatement();
                }
                catch (SyntaxErrorException e)
                {
                    Debug.LogError(e.Message);
                }
                catch (Exception e)
                {
                    throw new InternalErrorException("Parser error", e);
                }
            }

            return statements;
        }

        private static void ScanStatement()
        {
            List<Token> lineTokens = new();

            while (!IsAtEnd())
            {
                Token token = Advance();

                if (token.Type == TokenType.Newline)
                {
                    AddStatement(MatchStatement(lineTokens));
                    line++;
                    lineTokens.Clear();
                }
                else
                {
                    // Walk the Graph (tm)
                }
            }
        }

        private static AbstractStatement MatchStatement(List<Token> tokens)
        {
            throw new NotImplementedException();
        }

        private static Token Advance()
        {
            return source[current++];
        }

        private static bool MatchPattern(List<Token> tokens, params TokenType[] types)
        {
            if (tokens.Count != types.Length) return false;
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type != types[i])
                {
                    return false;
                }
            }

            current++;
            return true;
        }

        private static bool MatchNext(TokenType type)
        {
            if (IsAtEnd()) return false;
            if (Peek().Type != type) return false;

            current++;
            return true;
        } 
        
        private static Token Peek(int offset = 0) => IsAtEnd() ? new Token {Type = TokenType.EndOfFile} : source[current + offset];

        private static Token Peek(out Token token, int offset = 0)
        {
            token = Peek(offset);
            return token;
        }

        private static void AddStatement(AbstractStatement statement)
        {
            statements.Add(statement);
        }

        private static bool IsAtEnd() => current >= source.Count;
    }
}