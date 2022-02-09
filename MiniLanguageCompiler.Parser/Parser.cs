using System;
using MiniLanguageCompiler.Core;
using MiniLanguageCompiler.Core.Interfaces;
using MiniLanguageCompiler.Core.Models;

namespace MiniLanguageCompiler.Parser
{
    //LL(1)
    public class Parser : IParser
    {
        private readonly IScanner scanner;
        private readonly ILogger logger;
        private Token lookAhead;

        public Parser(IScanner scanner, ILogger logger)
        {
            this.scanner = scanner;
            this.logger = logger;
            this.Move();
        }

        public void Parse()
        {
            Program();
        }

        private void Program()
        {
            Block();
        }

        private void Block()
        {
            //{
            this.Match(TokenType.LeftBrace);
            Decls();
            Stmts();
            //}
            this.Match(TokenType.RightBrace);
        }

        private void Decls()
        {
            if (this.lookAhead.TokenType == TokenType.Identifier)
            {
                Decl();
                Decls();
            }
        }

        private void Decl()
        {
            //id
            this.Match(TokenType.Identifier);
            //:
            this.Match(TokenType.Colon);
            Type();
            //;
            this.Match(TokenType.Semicolon);
        }

        private void Type()
        {
            switch (this.lookAhead.TokenType)
            {
                case TokenType.NumberKeyword:
                    this.Match(TokenType.NumberKeyword);
                    break;
                case TokenType.StringKeyword:
                    this.Match(TokenType.StringKeyword);
                    break;
                case TokenType.ArrayKeyword:
                    this.Match(TokenType.ArrayKeyword);
                    this.Match(TokenType.LessThan);
                    Type();
                    this.Match(TokenType.GreaterThan);
                    break;
                default:
                    this.logger.Error($"Syntax error! Unrecognized type in line: {this.lookAhead.Line} and column: {this.lookAhead.Column}");
                    break;
            }
        }

        private void Stmts()
        {
            if (this.lookAhead.TokenType == TokenType.RightBrace)
            {
                return;
            }
            Stmt();
            Stmts();
        }

        private void Stmt()
        {
            switch (this.lookAhead.TokenType)
            {
                case TokenType.Identifier:
                    this.Match(TokenType.Identifier);
                    this.Match(TokenType.LessThan);
                    this.Match(TokenType.Minus);
                    AssignmentExpr();
                    this.Match(TokenType.Semicolon);
                    break;
                case TokenType.IfKeyword:
                    this.Match(TokenType.IfKeyword);
                    this.Match(TokenType.LeftParens);
                    LogicalOrExpr();
                    this.Match(TokenType.RightParens);
                    Stmt();
                    if (this.lookAhead.TokenType != TokenType.ElseKeyword)
                    {
                        break;
                    }
                    this.Match(TokenType.ElseKeyword);
                    Stmt();
                    break;
                case TokenType.WhileKeyword:
                    this.Match(TokenType.WhileKeyword);
                    this.Match(TokenType.LeftParens);
                    LogicalOrExpr();
                    this.Match(TokenType.RightParens);
                    Stmt();
                    break;
                case TokenType.PrintKeyword:
                    this.Match(TokenType.PrintKeyword);
                    this.Match(TokenType.LeftParens);
                    Params();
                    this.Match(TokenType.RightParens);
                    this.Match(TokenType.Semicolon);
                    break;
                default:
                    Block();
                    break;
            }
        }

        private void Params()
        {
            LogicalOrExpr();
            ParamsPrime();
        }

        private void ParamsPrime()
        {
            if (this.lookAhead.TokenType == TokenType.Comma)
            {
                this.Match(TokenType.Comma);
                LogicalOrExpr();
                ParamsPrime();
            }
        }

        private void AssignmentExpr()
        {
            if (this.lookAhead.TokenType == TokenType.LeftBracket)
            {
                this.Match(TokenType.LeftBracket);
                Params();
                this.Match(TokenType.RightBracket);
                return;
            }
            LogicalOrExpr();
        }

        private void LogicalOrExpr()
        {
            LogicalAndExpr();
            while (this.lookAhead.TokenType == TokenType.LogicalOr)
            {
                this.Move();
                LogicalAndExpr();
            }
        }

        private void LogicalAndExpr()
        {
            Eq();
            while (this.lookAhead.TokenType == TokenType.LogicalAnd)
            {
                this.Move();
                Eq();
            }
        }

        private void Eq()
        {
            Rel();
            while (this.lookAhead.TokenType == TokenType.Equal)
            {
                this.Move();
                Rel();
            }
        }


        private void Rel()
        {
            Expr();
            while(this.lookAhead.TokenType == TokenType.LessThan ||
                this.lookAhead.TokenType == TokenType.GreaterThan ||
                this.lookAhead.TokenType == TokenType.LessOrEqualThan ||
                this.lookAhead.TokenType == TokenType.GreaterOrEqualThan)
            {
                this.Move();
                Expr();
            }
        }

        private void Expr()
        {
            Term();
            while (this.lookAhead.TokenType == TokenType.Plus || this.lookAhead.TokenType == TokenType.Minus)
            {
                this.Move();
                Term();
            }
        }

        private void Term()
        {
            PostFixExpr();
            while (this.lookAhead.TokenType == TokenType.Multiplication || this.lookAhead.TokenType == TokenType.Division)
            {
                this.Move();
                PostFixExpr();
            }
        }

        private void PostFixExpr()
        {
            Factor();
            if (this.lookAhead.TokenType == TokenType.LeftBracket)
            {
                this.Match(TokenType.LeftBracket);
                LogicalOrExpr();
                this.Match(TokenType.RightBracket);
            }
        }

        private void Factor()
        {
            switch (this.lookAhead.TokenType)
            {
                case TokenType.LeftParens:
                    this.Match(TokenType.LeftParens);
                    LogicalOrExpr();
                    this.Match(TokenType.RightParens);
                    break;
                case TokenType.Identifier:
                    this.Match(TokenType.Identifier);
                    break;
                case TokenType.NumberLiteral:
                    this.Match(TokenType.NumberLiteral);
                    break;
                case TokenType.StringLiteral:
                    this.Match(TokenType.StringLiteral);
                    break;
                default:
                    break;
            }
        }
    
        private void Move()
        {
            this.lookAhead = this.scanner.GetNextToken();
        }

        private void Match(TokenType expectedTokenType)
        {
            if (this.lookAhead.TokenType != expectedTokenType)
            {
                this.logger.Error($"Syntax Error! expected token {expectedTokenType} but found {this.lookAhead.TokenType} on line {this.lookAhead.Line} and column {this.lookAhead.Column}");
                throw new ApplicationException($"Syntax Error! expected token {expectedTokenType} but found {this.lookAhead.TokenType} on line {this.lookAhead.Line} and column {this.lookAhead.Column}");
            }
            this.Move();
        }
    }
}
