using MiniLanguageCompiler.Core.Models;
using MiniLanguageCompiler.Core.Types;

namespace MiniLanguageCompiler.Core.Expressions
{
    public class ArrayAccessExpression : TypedExpression
    {
        public IdExpression Id { get; }

        public TypedExpression Index { get; }

        public ArrayAccessExpression(Type type, Token token, IdExpression id, TypedExpression index)
            : base(type, token)
        {
            Id = id;
            Index = index;
        }

        public override Type GetExpressionType()
        {
            return type;
        }
    }
}
