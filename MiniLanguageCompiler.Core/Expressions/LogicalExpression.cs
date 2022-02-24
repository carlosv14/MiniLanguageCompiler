using System;
using System.Collections.Generic;
using MiniLanguageCompiler.Core.Models;

namespace MiniLanguageCompiler.Core.Expressions
{
    public class LogicalExpression : BinaryExpression
    {
        private readonly Dictionary<(Type, Type), Type> _typeRules;

        public LogicalExpression(Token token, TypedExpression leftExpression, TypedExpression rightExpression)
            : base(token, leftExpression, rightExpression, null)
        {
            _typeRules = new Dictionary<(Type, Type), Type>
            {
                { (Type.Bool, Type.Bool), Type.Bool},
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

            throw new ApplicationException($"Cannot perform logical operation on types {leftType} and {rightType}");
        }
    }
}
