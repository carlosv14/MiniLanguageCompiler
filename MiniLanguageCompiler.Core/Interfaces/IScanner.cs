using MiniLanguageCompiler.Core.Models;

namespace MiniLanguageCompiler.Core.Interfaces
{
    public interface IScanner
    {
        Token GetNextToken();
    }
}
