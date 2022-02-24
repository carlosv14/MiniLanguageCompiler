using System;
using MiniLanguageCompiler.Core.Expressions;

namespace MiniLanguageCompiler.Core.Statements
{
    public class IfStatement : Statement
    {
        public IfStatement(TypedExpression expression, Statement trueStatement, Statement falseStatement)
        {
            Expression = expression;
            TrueStatement = trueStatement;
            FalseStatement = falseStatement;
        }

        public TypedExpression Expression { get; }
        public Statement TrueStatement { get; }
        public Statement FalseStatement { get; }

        public override void ValidateSemantic()
        {
            if (this.Expression.GetExpressionType() != Type.Bool)
            {
                throw new ApplicationException($"Expression inside if must be boolean");
            }
        }
    }
}
