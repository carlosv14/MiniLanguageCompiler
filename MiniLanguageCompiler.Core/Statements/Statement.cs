using System;
namespace MiniLanguageCompiler.Core.Statements
{
    public abstract class Statement
    {
        public abstract void ValidateSemantic();
    }
}
