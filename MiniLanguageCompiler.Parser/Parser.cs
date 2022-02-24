using System;
using System.Collections;
using System.Collections.Generic;
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
        private Environment topEnvironment;

        public Parser(IScanner scanner, ILogger logger)
        {
            this.scanner = scanner;
            this.logger = logger;
            this.Move();
        }

        public Statement Parse()
        {
            var program = Program();
            program.ValidateSemantic();
            return program;
        }

        private Statement Program()
        {
            return Block();
        }

        private Statement Block()
        {
            //{
            this.Match(TokenType.LeftBrace);
            var savedEnvironment = topEnvironment;
            topEnvironment = new Environment(topEnvironment);
            Decls();
            var statements = Stmts();
            //}
            this.Match(TokenType.RightBrace);
            topEnvironment = savedEnvironment;
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
            var token = this.lookAhead;
            this.Match(TokenType.Identifier);
            //:
            this.Match(TokenType.Colon);
            var type = Type();
            //;
            this.Match(TokenType.Semicolon);
            var id = new IdExpression(type, token);
            topEnvironment.Put(token.Lexeme, id);
        }

        private Core.Type Type()
        {
            switch (this.lookAhead.TokenType)
            {
                case TokenType.NumberKeyword:
                    this.Match(TokenType.NumberKeyword);
                    return Core.Type.Number;
                case TokenType.StringKeyword:
                    this.Match(TokenType.StringKeyword);
                    return Core.Type.String;
                case TokenType.ArrayKeyword:
                    this.Match(TokenType.ArrayKeyword);
                    this.Match(TokenType.LessThan);
                    Type();
                    this.Match(TokenType.GreaterThan);
                    return null;
                default:
                    throw new ApplicationException($"Syntax error! Unrecognized type in line: {this.lookAhead.Line} and column: {this.lookAhead.Column}");
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
                    var symbol = topEnvironment.Get(this.lookAhead.Lexeme);
                    this.Match(TokenType.Identifier);
                    this.Match(TokenType.LessThan);
                    this.Match(TokenType.Minus);
                    var stmt = AssignmentStmt(symbol.Id);
                    this.Match(TokenType.Semicolon);
                    return stmt;
                case TokenType.IfKeyword:
                    this.Match(TokenType.IfKeyword);
                    this.Match(TokenType.LeftParens);
                    var TypedExpression = LogicalOrExpr();
                    this.Match(TokenType.RightParens);
                    var trueStatement = Stmt();
                    if (this.lookAhead.TokenType != TokenType.ElseKeyword)
                    {
                        return new IfStatement(TypedExpression, trueStatement, null);
                    }
                    this.Match(TokenType.ElseKeyword);
                    var falseStatement = Stmt();
                    return new IfStatement(TypedExpression, trueStatement, falseStatement);
                case TokenType.WhileKeyword:
                    this.Match(TokenType.WhileKeyword);
                    this.Match(TokenType.LeftParens);
                    TypedExpression = LogicalOrExpr();
                    this.Match(TokenType.RightParens);
                    return new WhileStatement(TypedExpression, Stmt());
                case TokenType.PrintKeyword:
                    this.Match(TokenType.PrintKeyword);
                    this.Match(TokenType.LeftParens);
                    var @params = Params();
                    this.Match(TokenType.RightParens);
                    this.Match(TokenType.Semicolon);
                    return new PrintStatement(@params);
                default:
                    return Block();
            }
        }

        private IEnumerable<TypedExpression> Params()
        {
            var @params = new List<TypedExpression>();
            @params.Add(LogicalOrExpr());
            @params.AddRange(ParamsPrime());
            return @params;
        }

        private IEnumerable<TypedExpression> ParamsPrime()
        {
            var @params = new List<TypedExpression>();
            if (this.lookAhead.TokenType == TokenType.Comma)
            {
                this.Match(TokenType.Comma);
                @params.Add(LogicalOrExpr());
                @params.AddRange(ParamsPrime());
            }
            return @params;
        }

        private Statement AssignmentStmt(IdExpression id)
        {
            if (this.lookAhead.TokenType == TokenType.LeftBracket)
            {
                this.Match(TokenType.LeftBracket);
                Params();
                this.Match(TokenType.RightBracket);
                return null;
            }
            var expr = LogicalOrExpr();
            return new AssignationStatement(id, expr);
        }

        private TypedExpression LogicalOrExpr()
        {
            var TypedExpression = LogicalAndExpr();
            while (this.lookAhead.TokenType == TokenType.LogicalOr)
            {
                var token = this.lookAhead;
                this.Move();
                TypedExpression =  new LogicalExpression(token, TypedExpression, LogicalAndExpr());
            }

            return TypedExpression;
        }

        private TypedExpression LogicalAndExpr()
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

        private TypedExpression Eq()
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


        private TypedExpression Rel()
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

        private TypedExpression Expr()
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

        private TypedExpression Term()
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

        private TypedExpression PostFixExpr()
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

        private TypedExpression Factor()
        {
            switch (this.lookAhead.TokenType)
            {
                case TokenType.LeftParens:
                    this.Match(TokenType.LeftParens);
                    var expr = LogicalOrExpr();
                    this.Match(TokenType.RightParens);
                    return expr;
                case TokenType.NumberLiteral:
                    var token = this.lookAhead;
                    this.Match(TokenType.NumberLiteral);
                    return new ConstantExpression(Core.Type.Number, token);
                case TokenType.StringLiteral:
                    token = this.lookAhead;
                    this.Match(TokenType.StringLiteral);
                    return new ConstantExpression(Core.Type.String, token);
                default:
                    token = this.lookAhead;
                    this.Match(TokenType.Identifier);
                    return topEnvironment.Get(token.Lexeme).Id;
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
