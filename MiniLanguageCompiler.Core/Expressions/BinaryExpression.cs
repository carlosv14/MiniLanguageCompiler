using MiniLanguageCompiler.Core.Models;
using MiniLanguageCompiler.Core.Types;

namespace MiniLanguageCompiler.Core.Expressions
{
    public abstract class BinaryExpression : TypedExpression
    {
        public TypedExpression LeftExpression { get; }

        public TypedExpression RightExpression { get; }

        public BinaryExpression(Token token, TypedExpression leftExpression, TypedExpression rightExpression, Type type)
            :base(type, token)
        {
            LeftExpression = leftExpression;
            RightExpression = rightExpression;
        }
    }
}
