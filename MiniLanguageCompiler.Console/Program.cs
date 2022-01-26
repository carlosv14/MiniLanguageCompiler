using System;
using System.IO;
using MiniLanguageCompiler.Infrastructure;
using MiniLanguageCompiler.Lexer;

namespace MiniLanguageCompiler.Console
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var fileContent = File.ReadAllText("test.txt");
            var logger = new Logger();
            var scanner = new Scanner(new Input(fileContent), logger);
            var token = scanner.GetNextToken();
            while (token.TokenType != Core.TokenType.EOF)
            {
                logger.Info(token.ToString());
                token = scanner.GetNextToken();
            }
        }
    }
}
