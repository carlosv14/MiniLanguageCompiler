﻿using System;
using MiniLanguageCompiler.Core.Expressions;

namespace MiniLanguageCompiler.Core.Statements
{
    public class AssignationStatement : Statement
    {
        public AssignationStatement(IdExpression id, TypedExpression expression)
        {
            Id = id;
            Expression = expression;
        }

        public IdExpression Id { get; }
        public TypedExpression Expression { get; }

        public override void ValidateSemantic()
        {
            if (this.Id.GetExpressionType() != Expression.GetExpressionType())
            {
                throw new ApplicationException($"Type {Expression.GetExpressionType()} is not assignable to {Id.GetExpressionType()}");
            }
        }
    }
}
