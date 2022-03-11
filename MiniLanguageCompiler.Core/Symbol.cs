using System;
using MiniLanguageCompiler.Core.Expressions;

namespace MiniLanguageCompiler.Core
{
    public class Symbol
    {
        public Symbol(IdExpression id, dynamic value)
        {
            Id = id;
            Value = value;
        }

        public IdExpression Id { get; }

        public dynamic Value { get; set; }
    }
}
