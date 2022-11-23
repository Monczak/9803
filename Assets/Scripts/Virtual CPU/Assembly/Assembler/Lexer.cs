using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public static class Lexer
    {
        private static int start;
        private static int current;
        private static int line;

        private static List<Token> tokens;

        private static string sourceCode;
        
        public static List<Token> Lex(string sourceCode)
        {
            tokens = new List<Token>(); 
            start = 0;
            current = 0;
            line = 1;
            Lexer.sourceCode = sourceCode;

            while (!IsAtEnd())
            {
                start = current;
                ScanToken();
            }
            
            tokens.Add(new Token
            {
                Type = TokenType.EndOfFile,
                Content = "",
                Line = line,
                Literal = null
            });
            
            return tokens;
        }

        private static void ScanToken()
        {
            void DoNothing() {}
            
            char c = Advance();

            // TODO: several people think this is bad, but I like it, consider reconsidering
            (c switch
            {
                '\0' => new Action(DoNothing),  // Won't match on anything, but it's necessary
                                                // for the switch expression to know the return type
                
                '(' => () => AddToken(TokenType.LeftParen),
                ')' => () => AddToken(TokenType.RightParen),
                '#' => () => AddToken(TokenType.ImmediateOp),
                '$' => () => AddToken(TokenType.HexOp),
                '%' => () => AddToken(TokenType.BinaryOp),
                ',' => () => AddToken(TokenType.Comma),

                _ when char.IsWhiteSpace(c) => DoNothing,
                '\n' => () => line++,

                _ => () => throw new SyntaxErrorException("Unexpected character", c, line)
            })();
        }

        private static char Advance()
        {
            return sourceCode[current++];
        }

        private static void AddToken(TokenType type, object literal = null)
        {
            string text = sourceCode[start..current];
            tokens.Add(new Token
            {
                Content = text,
                Line = line,
                Literal = literal,
                Type = type
            });
        }

        private static bool IsAtEnd() => current >= sourceCode.Length;

    }
}