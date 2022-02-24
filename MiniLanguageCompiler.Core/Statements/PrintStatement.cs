using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MiniLanguageCompiler.Core.Expressions;

namespace MiniLanguageCompiler.Core.Statements
{
    public class PrintStatement : Statement
    {
        public PrintStatement(IEnumerable<TypedExpression> parameters)
        {
            Parameters = parameters;
        }

        public IEnumerable<TypedExpression> Parameters { get; }

        public override void ValidateSemantic()
        {
            if (Parameters.Any(x => x.GetExpressionType() != Type.String))
            {
                throw new ApplicationException("All parameters for print method must be string");
            }
        }
    }
}
