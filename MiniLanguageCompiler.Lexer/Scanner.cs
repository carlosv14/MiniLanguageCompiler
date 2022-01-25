using System;
using System.Collections.Generic;
using System.Text;
using MiniLanguageCompiler.Core;
using MiniLanguageCompiler.Core.Interfaces;
using MiniLanguageCompiler.Core.Models;

namespace MiniLanguageCompiler.Lexer
{
    public class Scanner : IScanner
    {
        private Input input;
        private readonly Dictionary<string, TokenType> keywords;

        public Scanner(Input input)
        {
            this.input = input;
            this.keywords = new Dictionary<string, TokenType>
            {
                ["if"] = TokenType.IfKeyword,
                ["else"] = TokenType.ElseKeyword,
                ["number"] = TokenType.NumberKeyword,
                ["string"] = TokenType.StringKeyword,
                ["while"] = TokenType.WhileKeyword,
            };
        }

        public Token GetNextToken()
        {
            var lexeme = new StringBuilder();
            var currentChar = this.GetNextChar();
            while (true)
            {
                while (char.IsWhiteSpace(currentChar) || currentChar == '\n')
                {
                    currentChar = this.GetNextChar();
                }

                if (char.IsLetter(currentChar))
                {
                    lexeme.Append(currentChar);
                    currentChar = this.PeekNextChar();
                    while (char.IsLetterOrDigit(currentChar))
                    {
                        currentChar = this.GetNextChar();
                        lexeme.Append(currentChar);
                        currentChar = this.PeekNextChar();
                    }

                    var tokenLexeme = lexeme.ToString();
                    if (this.keywords.ContainsKey(tokenLexeme))
                    {
                        return new Token
                        {
                            Column = this.input.Position.Column,
                            Line = this.input.Position.Line,
                            Lexeme = tokenLexeme,
                            TokenType = this.keywords[tokenLexeme]
                        };
                    }

                    return new Token
                    {
                        Column = this.input.Position.Column,
                        Line = this.input.Position.Line,
                        Lexeme = tokenLexeme,
                        TokenType = TokenType.Identifier,
                    };
                }
            }
        }

        private char GetNextChar()
        {
            var next = input.NextChar();
            input = next.Reminder;
            return next.Value;
        }

        private char PeekNextChar()
        {
            var next = input.NextChar();
            return next.Value;
        }
    }
}
