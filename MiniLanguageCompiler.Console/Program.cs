using System;
using System.IO;
using MiniLanguageCompiler.Lexer;

namespace MiniLanguageCompiler.Console
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var fileContent = File.ReadAllText("test.txt");
            var scanner = new Scanner(new Input(fileContent));
            System.Console.WriteLine(scanner.GetNextToken().ToString());
            System.Console.WriteLine(scanner.GetNextToken().ToString());
            System.Console.WriteLine(scanner.GetNextToken().ToString());
        }
    }
}
