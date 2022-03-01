using System;
using MiniLanguageCompiler.Core.Expressions;
using MiniLanguageCompiler.Core.Types;

namespace MiniLanguageCompiler.Core.Statements
{
    public class WhileStatement : Statement
    {
        public WhileStatement(TypedExpression expression, Statement statement)
        {
            Expression = expression;
            Statement = statement;
            this.ValidateSemantic();
        }

        public TypedExpression Expression { get; }
        public Statement Statement { get; }

        public override void ValidateSemantic()
        {
            if (this.Expression.GetExpressionType() != Types.Type.Bool)
            {
                throw new ApplicationException($"Expression inside while must be boolean");
            }
        }
    }
}
