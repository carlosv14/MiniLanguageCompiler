using System;
using MiniLanguageCompiler.Core.Expressions;
using MiniLanguageCompiler.Core.Types;

namespace MiniLanguageCompiler.Core.Statements
{
    //a[0] = 5
    public class ArrayAssignationStatement : Statement
    {
        public IdExpression Id { get; }

        public TypedExpression Index { get; }

        public TypedExpression Expression { get; }

        private readonly TypedExpression _access;

        public ArrayAssignationStatement(ArrayAccessExpression access, TypedExpression expression)
        {
            Id = access.Id;
            Index = access.Index;
            Expression = expression;
            _access = access;
            this.ValidateSemantic();
        }

        public override void ValidateSemantic()
        {
            //arr[1] = arr2;
            if (_access.GetExpressionType() is Core.Types.Array || Expression.GetExpressionType() is Core.Types.Array)
            {
               // throw new ApplicationException($"Type {Expression.GetExpressionType()} is not assignable to {Id.GetExpressionType()}");
            }
            else if (_access.GetExpressionType() != Expression.GetExpressionType())
            {
                throw new ApplicationException($"Type {Expression.GetExpressionType()} is not assignable to {Id.GetExpressionType()}");
            }
        }
    }
}
