using System;
using MiniLanguageCompiler.Core.Statements;

namespace MiniLanguageCompiler.Core.Interfaces
{
    public interface IParser
    {
        Statement Parse();
    }
}
