using MiniLanguageCompiler.Core.Models;
using MiniLanguageCompiler.Core.Types;

namespace MiniLanguageCompiler.Core.Expressions
{
    public class IdExpression : TypedExpression
    {
        public IdExpression(Type type, Token token)
            : base(type, token)
        {
        }

        public override dynamic Evaluate()
        {
            return EnvironmentManager.GetSymbolForInterpretation(this.Token.Lexeme).Value;
        }

        public override string GenerateCode()
        {
            return Token.Lexeme;
        }

        public override Type GetExpressionType()
        {
            return type;
        }
    }
}
