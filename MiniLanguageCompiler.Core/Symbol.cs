using System;
using MiniLanguageCompiler.Core.Expressions;

namespace MiniLanguageCompiler.Core
{
    public class Symbol
    {
        public Symbol(IdExpression id)
        {
            Id = id;
        }

        public IdExpression Id { get; }
    }
}
