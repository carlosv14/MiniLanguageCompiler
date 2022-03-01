using System;
namespace MiniLanguageCompiler.Core.Types
{
    public class Array : Type
    {
        public Type Of { get; }

        public Array(string lexeme, TokenType tokenType, Type of)
            : base(lexeme, tokenType)
        {
            Of = of;
        }
    }
}
