using System.Collections.Generic;
using MiniLanguageCompiler.Core.Models;
using MiniLanguageCompiler.Core.Types;

namespace MiniLanguageCompiler.Core.Expressions
{
    public class RelationalExpression : BinaryExpression
    {
        private readonly Dictionary<(Type, Type), Type> _typeRules;

        public RelationalExpression(Token token, TypedExpression leftExpression, TypedExpression rightExpression)
            : base(token, leftExpression, rightExpression, null)
        {
            _typeRules = new Dictionary<(Type, Type), Type>
            {
                { (Type.Number, Type.Number),  Type.Bool},
                { (Type.String, Type.String),  Type.Bool},
            };
        }

        public override Type GetExpressionType()
        {
            var leftType = LeftExpression.GetExpressionType();
            var rightType = RightExpression.GetExpressionType();
            if (_typeRules.TryGetValue((leftType, rightType), out var resultType))
            {
                return resultType;
            }

            throw new System.ApplicationException($"Cannot perform relational operation on types {leftType} and {rightType}");
        }
    }
}
