using System;
using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class Lexer : LogErrorProducer
    {
        private int start;
        private int current;
        private int line;

        private List<Token> tokens;

        private string sourceCode;

        private TokenType? expectedToken;
        private TokenType? lastToken;

        private bool expectingToken;

        public bool HadError { get; private set; }

        private enum NumberBase
        {
            Binary,
            Decimal,
            Hex
        }

        private Dictionary<string, TokenType> Keywords => new()
        {
            { "a", TokenType.RegisterA },
            { "A", TokenType.RegisterA },
            { "x", TokenType.RegisterX },
            { "X", TokenType.RegisterX },
            { "y", TokenType.RegisterY },
            { "Y", TokenType.RegisterY },
        };

        public List<Token> Lex(string sourceCode)
        {
            tokens = new List<Token>(); 
            start = 0;
            current = 0;
            line = 1;
            this.sourceCode = sourceCode;
            expectedToken = null;
            expectingToken = false;
            HadError = false;
            
            while (!IsAtEnd())
            {
                start = current;
                try
                {
                    OperationResult result = ScanToken();
                    if (result.Failed)
                    {
                        MakeError(result.TheError);
                        HadError = true;
                    }
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

        private OperationResult ScanToken()
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
                
                case '.': LexDirective(); break;
                
                case '\n': AddToken(TokenType.Newline); line++; break;
                
                case var _ when char.IsWhiteSpace(c): break;
                
                case '#':
                    AddToken(TokenType.ImmediateOp);
                    Expect(TokenType.Number);
                    break;
                
                case var _ when IsDecimalDigit(c) || (c == '$' && IsDigit( Peek(), NumberBase.Hex)) || (c == '%' && IsDigit( Peek(), NumberBase.Binary)):
                    var result = LexNumber(c);
                    if (result.Failed) return result;
                    break;
                
                case var _ when IsAlpha(c): LexIdentifier(); break;
                
                default: return OperationResult.Error(LexicalErrors.UnexpectedCharacter(c, line));
            }

            switch (expectingToken)
            {
                case true when expectedToken is not null && expectedToken != lastToken:
                    if (lastToken != null)
                        return OperationResult.Error(LexicalErrors.ExpectedGot(c, line, expectedToken.Value,
                            lastToken.Value));
                    throw new InternalErrorException("Last token was null");

                case false when expectedToken is not null:
                    expectingToken = true;
                    break;
                default:
                    expectedToken = null;
                    expectingToken = false;
                    break;
            }
            
            return OperationResult.Success();
        }

        private void LexDirective()
        {
            while (IsAlphaNumeric(Peek())) Advance();
            AddToken(TokenType.Directive);
        }

        private void LexIdentifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            string text = sourceCode[start..current];
            TokenType type = Keywords.ContainsKey(text) ? Keywords[text] : TokenType.Identifier;

            AddToken(MatchNext(':') ? TokenType.LabelDecl : type);
        }

        private OperationResult LexNumber(char firstChar)
        {
            NumberBase numberBase = firstChar switch
            {
                '$' => NumberBase.Hex,
                '%' => NumberBase.Binary,
                _ => NumberBase.Decimal
            };

            while (IsDigit(Peek(), numberBase)) Advance();

            // TODO: Maybe do this without exceptions?
            try
            {
                // ReSharper disable once HeapView.BoxingAllocation
                AddToken(TokenType.Number, numberBase switch
                {
                    NumberBase.Decimal => Convert.ToUInt16(sourceCode[start..current]),
                    NumberBase.Binary => Convert.ToUInt16(sourceCode[(start + 1)..current], 2),
                    NumberBase.Hex => Convert.ToUInt16(sourceCode[(start + 1)..current], 16),
                    _ => throw new ArgumentOutOfRangeException(nameof(numberBase))
                });
            }
            catch (Exception)
            {
                return OperationResult.Error(LexicalErrors.InvalidNumber(sourceCode[start], line));
            }
            
            return OperationResult.Success();
        }

        private char Advance()
        {
            return sourceCode[current++];
        }

        private bool MatchNext(char c)
        {
            if (IsAtEnd()) return false;
            if (sourceCode[current] != c) return false;

            current++;
            return true;
        }

        private char Peek(int offset = 0) => IsAtEnd() ? '\0' : sourceCode[current + offset];

        private bool IsHexDigit(char c) => IsDecimalDigit(c) || c is >= 'a' and <= 'f' or >= 'A' and <= 'F';
        private bool IsDecimalDigit(char c) => c is >= '0' and <= '9';
        private bool IsBinaryDigit(char c) => c is '0' or '1';

        private bool IsDigit(char c, NumberBase @base) => @base switch
        {
            NumberBase.Binary => IsBinaryDigit(c),
            NumberBase.Decimal => IsDecimalDigit(c),
            NumberBase.Hex => IsHexDigit(c),
            _ => throw new ArgumentOutOfRangeException(nameof(@base), @base, null)
        };

        private bool IsAlpha(char c) => c is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or '_';

        private bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDecimalDigit(c);
        
        private void Expect(TokenType type)
        {
            expectedToken = type;
        }

        private void AddToken(TokenType type, object literal = null)
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

        private bool IsAtEnd() => current >= sourceCode.Length;
    }
}