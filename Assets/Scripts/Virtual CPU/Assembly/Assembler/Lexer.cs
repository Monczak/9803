using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using UnityEngine;
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

        private static TokenType? expectedToken;
        private static TokenType? lastToken;

        private static bool expectingToken;
        
        public static bool HadError { get; private set; }

        private enum NumberBase
        {
            Binary,
            Decimal,
            Hex
        }

        private static readonly Dictionary<string, TokenType> Keywords = new()
        {
            { "a", TokenType.RegisterA },
            { "A", TokenType.RegisterA },
            { "x", TokenType.RegisterX },
            { "X", TokenType.RegisterX },
            { "y", TokenType.RegisterY },
            { "Y", TokenType.RegisterY },
        };

        public static List<Token> Lex(string sourceCode)
        {
            tokens = new List<Token>(); 
            start = 0;
            current = 0;
            line = 1;
            Lexer.sourceCode = sourceCode;
            expectedToken = null;
            expectingToken = false;
            HadError = false;
            
            while (!IsAtEnd())
            {
                start = current;
                try
                {
                    ScanToken();
                }
                catch (LexicalErrorException e)
                {
                    Debug.LogError(e.Message);
                    HadError = true;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    throw new InternalErrorException("Internal error occurred", e);
                }
            }

            if (HadError)
                return null;
            
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
                
                case '\n': AddToken(TokenType.Newline); line++; break;
                
                case var _ when char.IsWhiteSpace(c): break;
                
                case '#':
                    AddToken(TokenType.ImmediateOp);
                    Expect(TokenType.Number);
                    break;
                
                case var _ when IsDecimalDigit(c) || (c == '$' && IsDigit( Peek(), NumberBase.Hex)) || (c == '%' && IsDigit( Peek(), NumberBase.Binary)):
                    LexNumber(c);
                    break;
                
                case var _ when IsAlpha(c): LexIdentifier(); break;
                
                default: throw new LexicalErrorException("Unexpected character", c, line);
            }

            switch (expectingToken)
            {
                case true when expectedToken is not null && expectedToken != lastToken:
                    throw new LexicalErrorException($"Expected {expectedToken.ToString()}, got {lastToken.ToString()}", c, line);
                case false when expectedToken is not null:
                    expectingToken = true;
                    break;
                default:
                    expectedToken = null;
                    expectingToken = false;
                    break;
            }
        }

        private static void LexIdentifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            string text = sourceCode[start..current];
            TokenType type = Keywords.ContainsKey(text) ? Keywords[text] : TokenType.Identifier;

            AddToken(MatchNext(':') ? TokenType.LabelDecl : type);
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

        private static bool IsAlpha(char c) => c is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or '_';

        private static bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDecimalDigit(c);
        
        private static void Expect(TokenType type)
        {
            expectedToken = type;
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
            lastToken = type;
        }

        private static bool IsAtEnd() => current >= sourceCode.Length;

    }
}