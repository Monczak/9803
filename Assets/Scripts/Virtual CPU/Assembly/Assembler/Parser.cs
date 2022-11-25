using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public static class Parser
    {
        private static int current;
        private static int line;

        private static List<AbstractStatement> statements;

        private static List<Token> source;
        
        public static List<AbstractStatement> Parse(List<Token> tokens)
        {
            statements = new List<AbstractStatement>();
            source = tokens;
            line = 1;
            current = 0;

            while (!IsAtEnd())
            {
                try
                {
                    ScanStatement();
                    line++;
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
            List<Token> tokens = new();
            while (Peek(out var token) is not { Type: TokenType.EndOfFile } and not { Type: TokenType.Newline })
            {
                tokens.Add(token);
                current++;
            }
            
            
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