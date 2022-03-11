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
            var parser = new Parser.Parser(scanner, logger);
            var code = parser.Parse();
            code.Interpret();
            File.WriteAllText("result.txt", code.GenerateCode());
        }
    }
}
