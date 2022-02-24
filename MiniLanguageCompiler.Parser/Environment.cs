using System;
using System.Collections.Generic;
using MiniLanguageCompiler.Core;
using MiniLanguageCompiler.Core.Expressions;

namespace MiniLanguageCompiler.Parser
{
    public class Environment
    {
        private readonly Dictionary<string, Symbol> _symbolTable;
        protected Environment Previous;

        public Environment(Environment previous)
        {
            Previous = previous;
            _symbolTable = new Dictionary<string, Symbol>();
        }

        public void Put(string lexeme, IdExpression id)
        {
            if (_symbolTable.ContainsKey(lexeme))
            {
                throw new ApplicationException($"Symbol {lexeme} was previously defined in this scope");
            }
            _symbolTable.Add(lexeme, new Symbol(id));
        }

        public Symbol Get(string lexeme)
        {
            for (var currentEnv = this; currentEnv != null; currentEnv = currentEnv.Previous)
            {
                if (currentEnv._symbolTable.TryGetValue(lexeme, out var symbol))
                {
                    return symbol;
                }
            }

            throw new ApplicationException($"Symbol {lexeme} doesn't exist in current context.");
        }
    }
}
