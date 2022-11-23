using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Microsoft.Cci;
using UnityEngine.UIElements;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public static class Lexer
    {
        private static int start;
        private static int current;
        private static int line;

        private static List<Token> tokens;

        private static string sourceCode;

        private enum NumberBase
        {
            Binary,
            Decimal,
            Hex
        }
        
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
                try
                {
                    ScanToken();
                }
                catch (Exception e)
                {
                    throw new InternalErrorException("Internal error occurred", e);
                }
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
            char c = Advance();

            switch (c) 
            {
                case '(': AddToken(TokenType.LeftParen); break;
                case ')': AddToken(TokenType.RightParen); break;
                case ',': AddToken(TokenType.Comma); break;

                case ';':
                    while (Peek() != '\n' && !IsAtEnd()) Advance();
                    break;
                
                case '\n': line++; break;
                
                case var _ when char.IsWhiteSpace(c): break;
                
                case '#':
                    AddToken(TokenType.ImmediateOp);
                    // Expect number
                    break;
                
                case var _ when IsDecimalDigit(c) || (c == '$' && IsDigit( Peek(), NumberBase.Hex)) || (c == '%' && IsDigit( Peek(), NumberBase.Binary)):
                    LexNumber(c);
                    break;
                
                default: throw new SyntaxErrorException("Unexpected character", c, line);
            }
        }

        private static void LexNumber(char firstChar)
        {
            NumberBase numberBase = firstChar switch
            {
                '$' => NumberBase.Hex,
                '%' => NumberBase.Binary,
                _ => NumberBase.Decimal
            };

            while (IsDigit(Peek(), numberBase)) Advance();
            
            // ReSharper disable once HeapView.BoxingAllocation
            AddToken(TokenType.Number, numberBase switch
            {
                NumberBase.Decimal => Convert.ToInt16(sourceCode[start..current]),
                NumberBase.Binary => Convert.ToInt16(sourceCode[(start+1)..current], 2),
                NumberBase.Hex => Convert.ToInt16(sourceCode[(start+1)..current], 16),
                _ => throw new ArgumentOutOfRangeException(nameof(numberBase))
            });
        }

        private static char Advance()
        {
            return sourceCode[current++];
        }

        private static bool MatchNext(char c)
        {
            if (IsAtEnd()) return false;
            if (sourceCode[current] != c) return false;

            current++;
            return true;
        }

        private static char Peek(int offset = 0) => IsAtEnd() ? '\0' : sourceCode[current + offset];

        private static bool IsHexDigit(char c) => IsDecimalDigit(c) || c is >= 'a' and <= 'f' or >= 'A' and <= 'F';
        private static bool IsDecimalDigit(char c) => c is >= '0' and <= '9';
        private static bool IsBinaryDigit(char c) => c is '0' or '1';

        private static bool IsDigit(char c, NumberBase @base) => @base switch
        {
            NumberBase.Binary => IsBinaryDigit(c),
            NumberBase.Decimal => IsDecimalDigit(c),
            NumberBase.Hex => IsHexDigit(c),
            _ => throw new ArgumentOutOfRangeException(nameof(@base), @base, null)
        };

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