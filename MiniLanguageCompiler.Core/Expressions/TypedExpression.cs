using System;
using MiniLanguageCompiler.Core.Models;

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
    }
}
