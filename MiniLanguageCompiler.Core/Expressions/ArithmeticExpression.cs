using System.Collections.Generic;
using MiniLanguageCompiler.Core.Models;
using MiniLanguageCompiler.Core.Types;

namespace MiniLanguageCompiler.Core.Expressions
{
    public class ArithmeticExpression : BinaryExpression
    {
        private readonly Dictionary<(Type, Type, TokenType), Type> _typeRules;

        public ArithmeticExpression(Token token, TypedExpression leftExpression, TypedExpression rightExpression)
            : base(token, leftExpression, rightExpression, null)
        {
            _typeRules = new Dictionary<(Type, Type, TokenType), Type>
            {
                { (Type.Number, Type.Number, TokenType.Plus), Type.Number },
                { (Type.Number, Type.String, TokenType.Plus), Type.String },
                { (Type.String, Type.Number, TokenType.Plus), Type.String },
                { (Type.String, Type.String, TokenType.Plus), Type.String },
                { (Type.Number, Type.Number, TokenType.Minus), Type.Number },
                { (Type.Number, Type.Number, TokenType.Multiplication), Type.Number },
                { (Type.Number, Type.Number, TokenType.Division), Type.Number },
                { (Type.Bool, Type.Bool, TokenType.Plus), Type.Bool },
            };
        }

        // a + b
        public override Type GetExpressionType()
        {
            var leftType = LeftExpression.GetExpressionType();
            var rightType = RightExpression.GetExpressionType();
            if (_typeRules.TryGetValue((leftType, rightType, Token.TokenType), out var resultType))
            {
                return resultType;
            }

            throw new System.ApplicationException($"Cannot perform {Token.Lexeme} operation on types {leftType} and {rightType}");
        }
    }
}
