using System;
using MiniLanguageCompiler.Core;
using MiniLanguageCompiler.Core.Expressions;
using MiniLanguageCompiler.Core.Interfaces;
using MiniLanguageCompiler.Core.Models;
using MiniLanguageCompiler.Core.Statements;

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

        public Statement Parse()
        {
            return Program();
        }

        private Statement Program()
        {
            return Block();
        }

        private Statement Block()
        {
            //{
            this.Match(TokenType.LeftBrace);
            Decls();
            var statements = Stmts();
            //}
            this.Match(TokenType.RightBrace);
            return statements;
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

        private Statement Stmts()
        {
            if (this.lookAhead.TokenType == TokenType.RightBrace)
            {
                return null;
            }

            return new SequenceStatement(Stmt(), Stmts());
        }

        private Statement Stmt()
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
                    var expression = LogicalOrExpr();
                    this.Match(TokenType.RightParens);
                    var trueStatement = Stmt();
                    if (this.lookAhead.TokenType != TokenType.ElseKeyword)
                    {
                        break;
                    }
                    this.Match(TokenType.ElseKeyword);
                    var falseStatement = Stmt();
                    return new IfStatement(expression, trueStatement, falseStatement);
                case TokenType.WhileKeyword:
                    this.Match(TokenType.WhileKeyword);
                    this.Match(TokenType.LeftParens);
                    expression = LogicalOrExpr();
                    this.Match(TokenType.RightParens);
                    return new WhileStatement(expression, Stmt());
                case TokenType.PrintKeyword:
                    this.Match(TokenType.PrintKeyword);
                    this.Match(TokenType.LeftParens);
                    Params();
                    this.Match(TokenType.RightParens);
                    this.Match(TokenType.Semicolon);
                    break;
                default:
                    return Block();
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

        private Expression LogicalOrExpr()
        {
            var expression = LogicalAndExpr();
            while (this.lookAhead.TokenType == TokenType.LogicalOr)
            {
                var token = this.lookAhead;
                this.Move();
                expression =  new LogicalExpression(token, expression, LogicalAndExpr());
            }

            return expression;
        }

        private Expression LogicalAndExpr()
        {
            var expr = Eq();
            while (this.lookAhead.TokenType == TokenType.LogicalAnd)
            {
                var token = this.lookAhead;
                this.Move();
                expr = new LogicalExpression(token, expr, Eq());
            }

            return expr;
        }

        private Expression Eq()
        {
            var expr = Rel();
            while (this.lookAhead.TokenType == TokenType.Equal)
            {
                var token = this.lookAhead;
                this.Move();
                expr = new RelationalExpression(token, expr, Rel());
            }

            return expr;
        }


        private Expression Rel()
        {
            var expr = Expr();
            while(this.lookAhead.TokenType == TokenType.LessThan ||
                this.lookAhead.TokenType == TokenType.GreaterThan ||
                this.lookAhead.TokenType == TokenType.LessOrEqualThan ||
                this.lookAhead.TokenType == TokenType.GreaterOrEqualThan)
            {
                var token = this.lookAhead;
                this.Move();
                expr = new RelationalExpression(token, expr, Expr());
            }

            return expr;
        }

        private Expression Expr()
        {
            var expr = Term();
            while (this.lookAhead.TokenType == TokenType.Plus || this.lookAhead.TokenType == TokenType.Minus)
            {
                var token = this.lookAhead;
                this.Move();
                expr = new ArithmeticExpression(token, expr, Term());
            }

            return expr;
        }

        private Expression Term()
        {
            var expr = PostFixExpr();
            while (this.lookAhead.TokenType == TokenType.Multiplication || this.lookAhead.TokenType == TokenType.Division)
            {
                var token = this.lookAhead;
                this.Move();
                expr = new ArithmeticExpression(token, expr, PostFixExpr());
            }

            return expr;
        }

        private Expression PostFixExpr()
        {
            var expr = Factor();
            if (this.lookAhead.TokenType == TokenType.LeftBracket)
            {
                this.Match(TokenType.LeftBracket);
                LogicalOrExpr();
                this.Match(TokenType.RightBracket);
            }

            return expr;
        }

        private Expression Factor()
        {
            switch (this.lookAhead.TokenType)
            {
                case TokenType.LeftParens:
                    this.Match(TokenType.LeftParens);
                    var expr = LogicalOrExpr();
                    this.Match(TokenType.RightParens);
                    return expr;
                case TokenType.NumberLiteral:
                    this.Match(TokenType.NumberLiteral);
                    return new ConstantExpression(Core.Type.Number, this.lookAhead);
                case TokenType.StringLiteral:
                    this.Match(TokenType.StringLiteral);
                    return new ConstantExpression(Core.Type.String, this.lookAhead);
                default:
                    this.Match(TokenType.Identifier);
                    return null;
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
