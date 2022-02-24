using System;
namespace MiniLanguageCompiler.Core.Statements
{
    public class SequenceStatement : Statement
    {
        public SequenceStatement(Statement firstStatement, Statement nextStatement)
        {
            FirstStatement = firstStatement;
            NextStatement = nextStatement;
        }

        public Statement FirstStatement { get; }
        public Statement NextStatement { get; }

        public override void ValidateSemantic()
        {
            this.FirstStatement?.ValidateSemantic();
            this.NextStatement?.ValidateSemantic();
        }
    }
}
