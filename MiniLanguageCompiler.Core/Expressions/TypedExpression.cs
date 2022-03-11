using MiniLanguageCompiler.Core.Models;
using MiniLanguageCompiler.Core.Types;

namespace MiniLanguageCompiler.Core.Expressions
{
    public abstract class TypedExpression : Node
    {
        protected readonly Type type;

        public Token Token { get; }

        public TypedExpression(Type type, Token token)
        {
            Token = token;
            this.type = type;
        }

        public abstract Type GetExpressionType();

        public abstract string GenerateCode();

        public abstract dynamic Evaluate();
    }
}
